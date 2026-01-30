using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.DTOs.Document;

namespace Survey.Services.Document
{
    public interface IDocumentSurveyService
    {
        Task<int> CreateDraftFromTemplateAsync(DocumentSurveyCreateRequestDto req);
        Task<DocumentSurveyDto> GetForRenderAsync(int documentId);
        Task<List<DocumentSurveyDto>> GetListAsync();
        Task<string> GenerateDocumentNoAsync();
        Task UpgradeDraftToLatestTemplateAsync(int documentId, CancellationToken ct = default);
        Task SaveDraftAsync(DocumentSurveySaveRequestDto req);
        Task SubmitAsync(int documentId);
        Task DeleteAsync(int documentId);
        Task<List<TemplateOptionDto>> GetTemplateOptionsAsync(int positionId);
        Task<DocumentSurveyPreviewDto> GetPreviewFromTemplateAsync(int templateHeaderId);
        Task<List<DocumentSurveyDto>> GetListForCurrentUserAsync(int userId, string role);
        Task ApproveAsync(int documentId);
    }
}