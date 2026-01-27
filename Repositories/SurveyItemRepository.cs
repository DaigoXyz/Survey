using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using Survey.Data;
using Survey.Repositories.IRepositories.ISurveyRepository;
using Survey.Entities.SurveyEntities;

namespace Survey.Repositories
{
    public class SurveyItemRepository : ISurveyItemRepository
    {
        private readonly AppDbContext _db;
        public SurveyItemRepository(AppDbContext db) => _db = db;

        public async Task<SurveyItem?> GetByIdAsync(int id, bool includeOptions = false, CancellationToken ct = default)
        {
            IQueryable<SurveyItem> q = _db.SurveyItems;
            if (includeOptions) q = q.Include(x => x.CheckboxOptions);
            return await q.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<int> GetNextSortOrderAsync(int headerId, CancellationToken ct = default)
        {
            var max = await _db.SurveyItems
                .Where(x => x.HeaderId == headerId)
                .MaxAsync(x => (int?)x.SortOrder, ct);

            return (max ?? 0) + 1;
        }
    }
}