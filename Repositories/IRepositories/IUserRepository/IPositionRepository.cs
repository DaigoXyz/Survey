using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.Repositories.IRepositories.IUserRepository
{
    public interface IPositionRepository
    {
        Task<bool> ExistsAsync(int id);
        Task<int> GetAnyIdAsync();
        Task<bool> AnyAsync();
        Task AddDefaultAsync();
        Task SaveChangesAsync();
    }
}