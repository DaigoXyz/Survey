using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.Entities.SurveyEntities;

namespace Survey.Repositories.IRepositories.ISurveyRepository
{
    public interface ISurveyHeaderRepository
    {
        Task<List<SurveyHeader>> ListAsync(CancellationToken ct = default);
        Task<SurveyHeader?> GetByIdAsync(int id, bool includeChildren = false, CancellationToken ct = default);

        Task<string?> GetLastTemplateCodeForYearMonthAsync(string yearMonth, CancellationToken ct = default);

        Task AddAsync(SurveyHeader header, CancellationToken ct = default);
        void Update(SurveyHeader header);
        void Remove(SurveyHeader header);
    }

}