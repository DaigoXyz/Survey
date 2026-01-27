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

        public Task<User?> GetByUsernameAsync(string username) =>
            _db.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Username == username);

        public Task<User?> GetByIdWithRoleAsync(int id) =>
            _db.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> UsernameExistsAsync(string username) =>
            _db.Users.AnyAsync(x => x.Username == username);

        public Task AddAsync(User user) => _db.Users.AddAsync(user).AsTask();

        public Task SaveChangesAsync() => _db.SaveChangesAsync();
    }
}