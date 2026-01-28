using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.Entities.DocumentEntities;

namespace Survey.Repositories.IRepositories.IDocumentRepository
{
    public interface IDocumentSurveyRepository
    {
        Task<DocumentSurvey?> GetDocumentAsync(int documentId);
        Task<DocumentSurvey?> GetDocumentWithAnswersAsync(int documentId);
        Task AddDocumentAsync(DocumentSurvey doc);
        Task SaveChangesAsync();
    }
}