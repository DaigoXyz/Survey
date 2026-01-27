using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.Entities;

namespace Survey.Repositories.IRepositories.IUserRepository
{
    public interface IUserRelationRepository
    {
        Task AddAsync(UserRelation relation);
        Task SaveChangesAsync();
        Task<List<UserRelation>> GetBySupervisorIdAsync(int supervisorId);
    }
}