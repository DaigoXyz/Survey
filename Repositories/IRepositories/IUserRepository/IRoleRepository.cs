using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.Entities;

namespace Survey.Repositories.IRepositories.IUserRepository
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string name);
        Task<bool> AnyAsync();
        Task AddRangeAsync(IEnumerable<Role> roles);
        Task SaveChangesAsync();
    }
}