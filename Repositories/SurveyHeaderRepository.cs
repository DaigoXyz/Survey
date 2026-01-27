using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Survey.Data;
using Survey.Entities.SurveyEntities;
using Survey.Repositories.IRepositories.ISurveyRepository;

namespace Survey.Repositories
{
    public class SurveyHeaderRepository : ISurveyHeaderRepository
    {
        private readonly AppDbContext _db;
        public SurveyHeaderRepository(AppDbContext db) => _db = db;

        public async Task<List<SurveyHeader>> ListAsync(CancellationToken ct = default)
            => await _db.SurveyHeaders.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync(ct);

        public async Task<SurveyHeader?> GetByIdAsync(int id, bool includeChildren = false, CancellationToken ct = default)
        {
            IQueryable<SurveyHeader> q = _db.SurveyHeaders;
            if (includeChildren)
            {
                q = q.Include(h => h.Items.OrderBy(i => i.SortOrder))
                     .ThenInclude(i => i.CheckboxOptions.OrderBy(o => o.Id));
            }
            return await q.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<string?> GetLastTemplateCodeForYearMonthAsync(string yearMonth, CancellationToken ct = default)
        {
            var prefix = $"TEMPLATE/{yearMonth}/";

            return await _db.SurveyHeaders
                .Where(x => x.TemplateCode.StartsWith(prefix))
                .OrderByDescending(x => x.TemplateCode)
                .Select(x => x.TemplateCode)
                .FirstOrDefaultAsync(ct);
        }

        public Task AddAsync(SurveyHeader header, CancellationToken ct = default)
            => _db.SurveyHeaders.AddAsync(header, ct).AsTask();

        public void Update(SurveyHeader header) => _db.SurveyHeaders.Update(header);
        public void Remove(SurveyHeader header) => _db.SurveyHeaders.Remove(header);
    }
}