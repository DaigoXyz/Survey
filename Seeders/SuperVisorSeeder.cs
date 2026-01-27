using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.Repositories.IRepositories.IUserRepository;
using Survey.Services.Password;
using EntityUser = Survey.Entities.User;

namespace Survey.Seeders
{
    public class SupervisorSeeder
    {
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IPositionRepository _positionRepo;
        private readonly IPasswordService _passwordService;

        public SupervisorSeeder(
            IUserRepository userRepo,
            IRoleRepository roleRepo,
            IPositionRepository positionRepo,
            IPasswordService passwordService)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _positionRepo = positionRepo;
            _passwordService = passwordService;
        }

        public async Task SeedAsync()
        {
            var supervisorRole = await _roleRepo.GetByNameAsync("Supervisor");
            if (supervisorRole is null)
                throw new Exception("Role 'Supervisor' belum ada. Jalankan RoleSeeder dulu.");

            if (!await _positionRepo.AnyAsync())
            {
                await _positionRepo.AddDefaultAsync();
                await _positionRepo.SaveChangesAsync();
            }

            var exists = await _userRepo.UsernameExistsAsync("supervisor");
            if (exists) return;

            var positionId = await _positionRepo.GetAnyIdAsync();

            var supervisor = new EntityUser
            {
                Username = "supervisor",
                RoleId = supervisorRole.Id,
                PositionId = positionId,
                PositionName = "Direktur Utama",
                CreatedAt = DateTime.UtcNow
            };

            supervisor.PasswordHash = _passwordService.Hash("password", supervisor);

            await _userRepo.AddAsync(supervisor);
            await _userRepo.SaveChangesAsync();
        }
    }
}