using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Survey.Data;
using Survey.Entities;
using Survey.Repositories.IRepositories.IUserRepository;

namespace Survey.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db) => _db = db;

        public Task<List<User>> GetAllWithRoleAsync() =>
            _db.Users
               .Include(u => u.Role)
               .OrderBy(u => u.Id)
               .ToListAsync();
               
        public Task<User?> GetByUsernameAsync(string username) =>
            _db.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Username == username);

        public Task<User?> GetByIdWithRoleAsync(int id) =>
            _db.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> UsernameExistsAsync(string username) =>
            _db.Users.AnyAsync(x => x.Username == username);

        public Task<List<User>> GetAllSupervisorsAsync() =>
            _db.Users.Include(u => u.Role)
                     .Where(u => u.Role.Name == "Supervisor")
                     .ToListAsync();
        public Task AddAsync(User user) => _db.Users.AddAsync(user).AsTask();
        public Task DeleteAsync(User user)
        {
            _db.Users.Remove(user);
            return Task.CompletedTask;
        }
        public Task SaveChangesAsync() => _db.SaveChangesAsync();
    }
}