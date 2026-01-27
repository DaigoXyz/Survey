using Microsoft.EntityFrameworkCore;
using Survey.Entities;
using Survey.Data;
using Survey.Repositories.IRepositories.IUserRepository;

namespace Survey.Repositories.RoleRepository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _db;

        public RoleRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<Role?> GetByNameAsync(string name)
        {
            return _db.Roles.FirstOrDefaultAsync(r => r.Name == name);
        }

        public Task<bool> AnyAsync()
        {
            return _db.Roles.AnyAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Role> roles)
        {
            await _db.Roles.AddRangeAsync(roles);
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
