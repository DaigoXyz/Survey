using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.Entities.SurveyEntities;

namespace Survey.Repositories.IRepositories.ISurveyRepository
{
    public interface ISurveyItemRepository
    {
        Task<SurveyItem?> GetByIdAsync(int id, bool includeOptions = false, CancellationToken ct = default);
        Task<int> GetNextSortOrderAsync(int headerId, CancellationToken ct = default);
    }
}