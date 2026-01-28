using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.DTOs.Document;

namespace Survey.Services.Document
{
    public interface IDocumentSurveyService
    {
        Task<int> CreateDraftFromTemplateAsync(string documentNo, DateTime documentDate, int surveyHeaderId);
        Task<DocumentSurveyDto> GetForRenderAsync(int documentId);

        Task SaveDraftAsync(DocumentSurveySaveRequestDto req);
        Task SubmitAsync(int documentId);

    }
}