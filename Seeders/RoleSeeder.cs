using Survey.Entities;
using Survey.Repositories.IRepositories.IUserRepository;

namespace Survey.Seeders
{
    public class RoleSeeder
    {
        private readonly IRoleRepository _roleRepo;

        public RoleSeeder(IRoleRepository roleRepo)
        {
            _roleRepo = roleRepo;
        }

        public async Task SeedAsync()
        {
            // idempotent: kalau sudah ada, skip
            if (await _roleRepo.AnyAsync())
                return;

            var now = DateTime.UtcNow;

            await _roleRepo.AddRangeAsync(new[]
            {
                new Role
                {
                    Name = "Supervisor",
                    Created_at = now
                },
                new Role
                {
                    Name = "User",
                    Created_at = now
                }
            });

            await _roleRepo.SaveChangesAsync();
        }
    }
}
