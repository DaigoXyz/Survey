using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Survey.DTOs.Survey;

namespace Survey.Services.Survey
{
    public interface ISurveyTemplateService
    {
        Task<SurveyHeaderDetailDto> CreateHeaderAsync(SurveyHeaderCreateDto dto, CancellationToken ct = default);
        Task<List<SurveyHeaderListDto>> ListAsync(CancellationToken ct = default);
        Task<SurveyHeaderDetailDto> GetDetailAsync(int headerId, CancellationToken ct = default);

        Task UpdateHeaderAsync(int headerId, SurveyHeaderUpdateDto dto, CancellationToken ct = default);
        Task DeleteHeaderAsync(int headerId, CancellationToken ct = default);

        Task<SurveyItemDto> AddItemAsync(int headerId, SurveyItemCreateDto dto, CancellationToken ct = default);
        Task UpdateItemAsync(int itemId, SurveyItemUpdateDto dto, CancellationToken ct = default);
        Task DeleteItemAsync(int itemId, CancellationToken ct = default);

        Task ReorderItemAsync(int headerId, ReorderItemDto dto, CancellationToken ct = default);
    }
}