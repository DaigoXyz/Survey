using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.Entities;

namespace Survey.Repositories.IRepositories.IUserRepository
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdWithRoleAsync(int id);
        Task<bool> UsernameExistsAsync(string username);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}