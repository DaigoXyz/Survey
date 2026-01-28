using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.Data;
using Survey.DTOs.Document;
using Survey.Entities.DocumentEntities;
using Survey.Entities.SurveyEntities;
using Microsoft.EntityFrameworkCore;
using Survey.Repositories.IRepositories.IDocumentRepository;

namespace Survey.Services.Document
{
    public class DocumentSurveyService : IDocumentSurveyService
    {
        private readonly AppDbContext _db;
        private readonly IDocumentSurveyRepository _repo;

        public DocumentSurveyService(AppDbContext db, IDocumentSurveyRepository repo)
        {
            _db = db;
            _repo = repo;
        }

        public async Task<int> CreateDraftFromTemplateAsync(string documentNo, DateTime documentDate, int surveyHeaderId)
        {
            var header = await _db.SurveyHeaders
                .Include(h => h.Items)
                    .ThenInclude(i => i.CheckboxOptions)
                .FirstOrDefaultAsync(h => h.Id == surveyHeaderId);

            if (header == null) throw new InvalidOperationException("Survey template tidak ditemukan.");

            var doc = new DocumentSurvey
            {
                DocumentNo = documentNo,
                DocumentDate = documentDate,
                Status = Status.Draft,
                Answers = header.Items
                    .OrderBy(i => i.SortOrder)
                    .Select(i => new DocumentSurveyAnswer
                    {
                        ItemId = i.Id,
                        QuestionSnapshot = i.Question,
                        TypeSnapshot = i.type,
                        SortOrderSnapshot = i.SortOrder,
                        CreatedAt = DateTime.UtcNow
                    })
                    .ToList()
            };

            await _repo.AddDocumentAsync(doc);
            await _repo.SaveChangesAsync();
            return doc.Id;
        }

        public async Task<DocumentSurveyDto> GetForRenderAsync(int documentId)
        {
            var doc = await _repo.GetDocumentWithAnswersAsync(documentId);
            if (doc == null) throw new InvalidOperationException("Document tidak ditemukan.");

            var readOnly = doc.Status != Status.Draft;

            var dto = new DocumentSurveyDto
            {
                Id = doc.Id,
                DocumentNo = doc.DocumentNo,
                DocumentDate = doc.DocumentDate,
                Status = (int)doc.Status,
                IsReadOnly = readOnly,
                Answers = doc.Answers
                    .OrderBy(a => a.SortOrderSnapshot)
                    .Select(a => new DocumentSurveyAnswerDto
                    {
                        AnswerId = a.Id,
                        Question = a.QuestionSnapshot,
                        Type = (int)a.TypeSnapshot,
                        SortOrder = a.SortOrderSnapshot,
                        Answer = a.Answer,

                        Options = (!readOnly && a.Item != null && a.Item.type == QuestionType.Checkbox)
                            ? a.Item.CheckboxOptions
                                .Select(o => new CheckboxOptionDto { Id = o.Id, Text = o.Name })
                                .ToList()
                            : new List<CheckboxOptionDto>(),

                        SelectedOptionIds = (!readOnly)
                            ? a.DocumentSurveyAnswerOptions
                                .Where(x => x.CheckboxOptionId.HasValue)
                                .Select(x => x.CheckboxOptionId!.Value)
                                .ToList()
                            : new List<int>(),

                        SelectedOptionTexts = (readOnly)
                            ? a.DocumentSurveyAnswerOptions
                                .Select(x => x.OptionTextSnapshot)
                                .Where(t => !string.IsNullOrWhiteSpace(t))
                                .ToList()
                            : new List<string>()
                    })
                    .ToList()
            };
            return dto;
        }

        public async Task SaveDraftAsync(DocumentSurveySaveRequestDto req)
        {
            var doc = await _repo.GetDocumentWithAnswersAsync(req.DocumentId);
            if (doc == null) throw new InvalidOperationException("Document tidak ditemukan.");
            if (doc.Status != Status.Draft) throw new InvalidOperationException("Document sudah tidak bisa diedit.");

            var answerById = doc.Answers.ToDictionary(a => a.Id);

            foreach (var input in req.Answers)
            {
                if (!answerById.TryGetValue(input.AnswerId, out var answer))
                    throw new InvalidOperationException($"AnswerId {input.AnswerId} tidak valid.");

                if (answer.TypeSnapshot != QuestionType.Checkbox)
                {
                    answer.Answer = input.Answer;
                    answer.UpdatedAt = DateTime.UtcNow;
                    continue;
                }

                answer.DocumentSurveyAnswerOptions.Clear();

                if (answer.Item == null)
                    throw new InvalidOperationException("Template item untuk checkbox sudah hilang, draft tidak bisa diubah.");

                var optionMap = answer.Item.CheckboxOptions.ToDictionary(o => o.Id);

                foreach (var optId in input.SelectedCheckboxOptionIds.Distinct())
                {
                    if (!optionMap.TryGetValue(optId, out var opt))
                        throw new InvalidOperationException($"CheckboxOptionId {optId} tidak valid.");

                    answer.DocumentSurveyAnswerOptions.Add(new DocumentSurveyAnswerOption
                    {
                        CheckboxOptionId = opt.Id,
                        OptionTextSnapshot = opt.Name
                    });
                }

                answer.UpdatedAt = DateTime.UtcNow;
            }

            await _repo.SaveChangesAsync();
        }

        public async Task SubmitAsync(int documentId)
        {
            var doc = await _repo.GetDocumentWithAnswersAsync(documentId);
            if (doc == null) throw new InvalidOperationException("Document tidak ditemukan.");
            if (doc.Status != Status.Draft) throw new InvalidOperationException("Document sudah pernah disubmit.");

            foreach (var a in doc.Answers)
            {
                if (string.IsNullOrWhiteSpace(a.QuestionSnapshot))
                {
                    if (a.Item == null) throw new InvalidOperationException("Template item hilang, tidak bisa freeze.");
                    a.QuestionSnapshot = a.Item.Question;
                    a.TypeSnapshot = a.Item.type;
                    a.SortOrderSnapshot = a.Item.SortOrder;
                }

                if (a.TypeSnapshot == QuestionType.Checkbox)
                {
                    foreach (var ao in a.DocumentSurveyAnswerOptions)
                    {
                        if (string.IsNullOrWhiteSpace(ao.OptionTextSnapshot))
                        {
                            if (ao.CheckboxOption != null)
                                ao.OptionTextSnapshot = ao.CheckboxOption.Name;
                            else if (ao.CheckboxOptionId.HasValue)
                                ao.OptionTextSnapshot = "Unknown option";
                        }
                    }
                }
            }

            doc.Status = Status.ConfirmToApprove;
            await _repo.SaveChangesAsync();
        }
    }        
}
