using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.DTOs.User;
using Survey.Repositories.IRepositories.IUserRepository;
using Survey.Mappers.IMappers;
using Survey.Entities;
using Survey.Services.Password;
using Survey.Services.User;
using UserEntity = Survey.Entities.User;

namespace Survey.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IPositionRepository _positionRepo;
        private readonly IUserRelationRepository _relationRepo;
        private readonly IUserMapper _mapper;
        private readonly IPasswordService _passwordService;

        public UserService(
            IUserRepository userRepo,
            IRoleRepository roleRepo,
            IPositionRepository positionRepo,
            IUserRelationRepository relationRepo,
            IUserMapper mapper,
            IPasswordService passwordService)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _positionRepo = positionRepo;
            _relationRepo = relationRepo;
            _mapper = mapper;
            _passwordService = passwordService;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAllWithRoleAsync();
            return users.Select(u => _mapper.ToDto(u)).ToList();
        }
        public async Task<UserDto> CreateUserAsync(int supervisorId, CreateUserDto dto)
        {
            // optional hard-check supervisor valid
            var supervisor = await _userRepo.GetByIdWithRoleAsync(supervisorId);
            if (supervisor is null) throw new UnauthorizedAccessException("Supervisor tidak valid.");
            if (!string.Equals(supervisor.Role.Name, "Supervisor", StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Bukan supervisor.");

            if (await _userRepo.UsernameExistsAsync(dto.Username))
                throw new InvalidOperationException("Username sudah dipakai.");

            if (!await _positionRepo.ExistsAsync(dto.PositionId))
                throw new KeyNotFoundException("PositionId tidak ditemukan.");

            var userRole = await _roleRepo.GetByNameAsync("User")
                ?? throw new InvalidOperationException("Role 'User' belum ada.");

            var entity = new UserEntity
            {
                Username = dto.Username,
                RoleId = userRole.Id,
                PositionId = dto.PositionId,
                PositionName = dto.PositionName,
                CreatedAt = DateTime.UtcNow
            };

            entity.PasswordHash = _passwordService.Hash("password", entity);

            await _userRepo.AddAsync(entity);
            await _userRepo.SaveChangesAsync();

            var relation = new UserRelation
            {
                SupervisorId = supervisorId,
                UserId = entity.Id
            };

            await _relationRepo.AddAsync(relation);
            await _relationRepo.SaveChangesAsync();

            // biar Role kebaca di mapper (atau reload)
            var created = await _userRepo.GetByIdWithRoleAsync(entity.Id) ?? entity;

            return _mapper.ToDto(created);
        }

        public async Task<List<UserDto>> GetUserAsync(int supervisorId)
        {
            var relations = await _relationRepo.GetBySupervisorIdAsync(supervisorId);
            return relations.Select(r => _mapper.ToDto(r.User)).ToList();
        }
        
        public async Task<UserDto> UpdateUserAsync(int supervisorId, UpdateUserDto dto)
        {
            var user = await _userRepo.GetByIdWithRoleAsync(dto.Id)
                ?? throw new KeyNotFoundException("User tidak ditemukan.");

            // optional: validasi supervisor
            if (user.Id == supervisorId)
                throw new InvalidOperationException("Tidak bisa mengupdate diri sendiri.");

            user.PositionId = dto.PositionId;
            user.PositionName = dto.PositionName;

            await _userRepo.SaveChangesAsync();
            return _mapper.ToDto(user);
        }

    }
}