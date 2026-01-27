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
    public class UserRelationRepository : IUserRelationRepository
    {
        private readonly AppDbContext _db;
        public UserRelationRepository(AppDbContext db) => _db = db;

        public Task AddAsync(UserRelation relation) => _db.UserRelations.AddAsync(relation).AsTask();
        public Task SaveChangesAsync() => _db.SaveChangesAsync();

        public Task<List<UserRelation>> GetBySupervisorIdAsync(int supervisorId) =>
            _db.UserRelations
               .Include(r => r.User).ThenInclude(u => u.Role)
               .Where(r => r.SupervisorId == supervisorId)
               .ToListAsync();

        public async Task DeleteByUserIdAsync(int userId)
        {
            var rels = await _db.UserRelations.Where(r => r.UserId == userId).ToListAsync();
            _db.UserRelations.RemoveRange(rels);
        }
    }
}