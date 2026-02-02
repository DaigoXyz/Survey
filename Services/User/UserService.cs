using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.DTOs.User;
using Survey.Repositories.IRepositories.IUserRepository;
using Survey.Mappers.IMappers;
using Survey.Entities;
using Survey.Services.Password;
using Survey.DTOs.Document;
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
        private readonly IPositionService _positionService;

        public UserService(
            IUserRepository userRepo,
            IRoleRepository roleRepo,
            IPositionRepository positionRepo,
            IUserRelationRepository relationRepo,
            IUserMapper mapper,
            IPasswordService passwordService,
            IPositionService positionService
            )
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _positionRepo = positionRepo;
            _relationRepo = relationRepo;
            _mapper = mapper;
            _passwordService = passwordService;
            _positionService = positionService;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAllWithRoleAsync();
            return users.Select(u => _mapper.ToDto(u)).ToList();
        }
        public async Task<UserDto> CreateUserAsync(int supervisorId, CreateUserDto dto)
        {
            var supervisor = await _userRepo.GetByIdWithRoleAsync(supervisorId);
            if (supervisor is null) throw new UnauthorizedAccessException("Supervisor tidak valid.");
            // if (!string.Equals(supervisor.Role.Name, "Supervisor", StringComparison.OrdinalIgnoreCase))
            //     throw new UnauthorizedAccessException("Bukan supervisor.");

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
                SupervisorId = dto.SupervisorId,
                UserId = entity.Id
            };


            await _relationRepo.AddAsync(relation);
            await _relationRepo.SaveChangesAsync();

            var created = await _userRepo.GetByIdWithRoleAsync(entity.Id) ?? entity;

            return _mapper.ToDto(created);
        }
        public async Task<List<UserDto>> GetUserAsync(int supervisorId)
        {
            var relations = await _relationRepo.GetBySupervisorIdAsync(supervisorId);
            return relations.Select(r => _mapper.ToDto(r.User)).ToList();
        }

        public async Task<RequesterProfileDto> GetRequesterProfileAsync(int userId)
        {
            var user = await _userRepo.GetByIdWithRoleAsync(userId)
                ?? throw new KeyNotFoundException("User tidak ditemukan.");

            var rel = await _relationRepo.GetByUserIdAsync(userId);

            var supervisor = rel?.Supervisor;
            return new RequesterProfileDto
            {
                UserId = user.Id,
                Username = user.Username,

                EmployeeId = user.Id.ToString(),
                EmployeeName = user.Username,

                PositionLevel = await _positionService.GetPositionNameAsync(user.PositionId),
                PositionName = user.PositionName,

                SupervisorId = supervisor?.Id.ToString() ?? "",
                SupervisorName = supervisor?.Username ?? ""
            };
        }

        public async Task<UserDto> UpdateUserAsync(int supervisorId, UpdateUserDto dto)
        {
            var user = await _userRepo.GetByIdWithRoleAsync(dto.Id)
                ?? throw new KeyNotFoundException("User tidak ditemukan.");

            if (user.Id == supervisorId)
                throw new InvalidOperationException("Tidak bisa mengupdate diri sendiri.");

            if (dto.SupervisorId <= 0)
                throw new InvalidOperationException("Supervisor wajib dipilih.");

            if (dto.SupervisorId == dto.Id)
                throw new InvalidOperationException("Supervisor tidak boleh diri sendiri.");

            var sup = await _userRepo.GetByIdWithRoleAsync(dto.SupervisorId);
            if (sup is null)
                throw new KeyNotFoundException("Supervisor tidak ditemukan.");

            user.PositionId = dto.PositionId;
            user.PositionName = dto.PositionName;
            user.Username = dto.Username;

            await _userRepo.SaveChangesAsync();

            await _relationRepo.DeleteByUserIdAsync(user.Id);
            await _relationRepo.SaveChangesAsync();

            await _relationRepo.AddAsync(new UserRelation
            {
                UserId = user.Id,
                SupervisorId = dto.SupervisorId
            });
            await _relationRepo.SaveChangesAsync();

            return _mapper.ToDto(user);
        }

        public async Task DeleteUserAsync(int supervisorId, int userId)
        {
            var supervisor = await _userRepo.GetByIdWithRoleAsync(supervisorId);
            if (supervisor is null) throw new UnauthorizedAccessException("Supervisor tidak valid.");
            // if (!string.Equals(supervisor.Role.Name, "Supervisor", StringComparison.OrdinalIgnoreCase))
            //     throw new UnauthorizedAccessException("Bukan supervisor.");

            var user = await _userRepo.GetByIdWithRoleAsync(userId)
                ?? throw new KeyNotFoundException("User tidak ditemukan.");

            if (string.Equals(user.Role.Name, "Supervisor", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Tidak boleh menghapus Supervisor.");

            await _relationRepo.DeleteByUserIdAsync(userId);   
            await _relationRepo.SaveChangesAsync();

            await _userRepo.DeleteAsync(user); 
            await _userRepo.SaveChangesAsync();
        }

        public async Task<bool> IsSupervisorOfUserAsync(int supervisorId, int userId)
        {
            var rel = await _relationRepo.GetByUserIdAsync(userId);
            return rel != null && rel.SupervisorId == supervisorId;
        }
    }
}