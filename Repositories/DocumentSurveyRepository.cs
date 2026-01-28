using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Survey.Data;
using Survey.Entities.DocumentEntities;
using Survey.Repositories.IRepositories.IDocumentRepository;


namespace Survey.Repositories
{
    public class DocumentSurveyRepository : IDocumentSurveyRepository
    {
        private readonly AppDbContext _db;
        public DocumentSurveyRepository(AppDbContext db) => _db = db;

        public Task<DocumentSurvey?> GetDocumentAsync(int documentId)
            => _db.DocumentSurveys.FirstOrDefaultAsync(x => x.Id == documentId);

        public Task<DocumentSurvey?> GetDocumentWithAnswersAsync(int documentId)
            => _db.DocumentSurveys
                .Include(d => d.Answers)
                    .ThenInclude(a => a.DocumentSurveyAnswerOptions)
                        .ThenInclude(o => o.CheckboxOption)
                .Include(d => d.Answers)
                    .ThenInclude(a => a.Item)
                        .ThenInclude(i => i.CheckboxOptions)
                .FirstOrDefaultAsync(x => x.Id == documentId);

        public Task AddDocumentAsync(DocumentSurvey doc)
            => _db.DocumentSurveys.AddAsync(doc).AsTask();

        public Task SaveChangesAsync()
            => _db.SaveChangesAsync();
    }
}