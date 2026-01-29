using Microsoft.EntityFrameworkCore;
using Survey.Entities;
using Survey.Data;
using Survey.Repositories.IRepositories.IUserRepository;

namespace Survey.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly AppDbContext _db;

        public PositionRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<List<Position>> GetAllAsync()
        {
            return _db.Positions
                      .OrderBy(p => p.Id)
                      .ToListAsync();
        }

        public Task<bool> ExistsAsync(int id)
        {
            return _db.Positions.AnyAsync(p => p.Id == id);
        }

        public Task<Position?> GetByIdAsync(int id)
        {
            return _db.Positions.FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<bool> AnyAsync()
        {
            return _db.Positions.AnyAsync();
        }

        public async Task<int> GetAnyIdAsync()
        {
            var id = await _db.Positions.Select(p => p.Id).FirstOrDefaultAsync();
            if (id == 0)
                throw new InvalidOperationException("Tidak ada data Position. Jalankan seeder dulu.");
            return id;
        }

        public async Task AddDefaultAsync()
        {
            if (await _db.Positions.AnyAsync())
                return;

            var now = DateTime.UtcNow;

            await _db.Positions.AddRangeAsync(new[]
            {
                new Position
                {
                    Name = "Director",
                    Created_at = now
                },
                new Position
                {
                    Name = "Head Department",
                    Created_at = now
                },
                new Position
                {
                    Name = "Officer",
                    Created_at = now
                }
            });
        }


        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
