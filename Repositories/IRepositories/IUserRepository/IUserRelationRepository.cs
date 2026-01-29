using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.Entities;

namespace Survey.Repositories.IRepositories.IUserRepository
{
    public interface IUserRelationRepository
    {
        Task<List<UserRelation>> GetBySupervisorIdAsync(int supervisorId);
        Task<UserRelation?> GetByUserIdAsync(int userId);
        Task DeleteByUserIdAsync(int userId);
        Task AddAsync(UserRelation relation);
        Task SaveChangesAsync();
    }
}