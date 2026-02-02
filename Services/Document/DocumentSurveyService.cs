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
using Survey.Services.User;

namespace Survey.Services.Document
{
    public class DocumentSurveyService : IDocumentSurveyService
    {
        private readonly AppDbContext _db;
        private readonly IDocumentSurveyRepository _repo;
        private readonly IUserService _userService;

        public DocumentSurveyService(AppDbContext db, IDocumentSurveyRepository repo, IUserService userService)
        {
            _db = db;
            _repo = repo;
            _userService = userService;
        }

        public async Task<int> CreateDraftFromTemplateAsync(DocumentSurveyCreateRequestDto req)
        {
            var header = await _db.SurveyHeaders
                .Include(h => h.Items)
                    .ThenInclude(i => i.CheckboxOptions)
                .FirstOrDefaultAsync(h => h.Id == req.TemplateHeaderId);

            if (header == null) throw new InvalidOperationException("Survey template tidak ditemukan.");

            var doc = new DocumentSurvey
            {
                DocumentNo = req.DocumentNo,
                DocumentDate = req.DocumentDate,
                Status = Status.New,

                CreatedByUserId = req.CreatedByUserId,
                CreatedByUserName = req.CreatedByUserName,
                CreatedAt = DateTime.Now,

                RequesterEmployeeId = req.RequesterEmployeeId,
                RequesterEmployeeName = req.RequesterEmployeeName,
                RequesterPositionLevel = req.RequesterPositionLevel,
                RequesterPositionName = req.RequesterPositionName,
                SupervisorId = req.SupervisorId,
                SupervisorName = req.SupervisorName,

                TemplateHeaderId = header.Id,
                StructuralVersionSnapshot = header.StructuralVersion,

                TemplateCodeSnapshot = header.TemplateCode,
                TemplateNameSnapshot = header.TemplateName,
                ThemeSnapshot = header.Theme,

                Answers = header.Items
                .OrderBy(i => i.SortOrder)
                .Select(i => new DocumentSurveyAnswer
                {
                    ItemId = i.Id,
                    QuestionSnapshot = i.Question,
                    TypeSnapshot = i.type,
                    SortOrderSnapshot = i.SortOrder,
                    CreatedAt = DateTime.Now
                })
                .ToList()
            };


            await _repo.AddDocumentAsync(doc);
            await _repo.SaveChangesAsync();
            return doc.Id;
        }
        public async Task UpgradeDraftToLatestTemplateAsync(int documentId, CancellationToken ct)
        {
            var doc = await _db.DocumentSurveys
                .Include(d => d.Answers)
                    .ThenInclude(a => a.DocumentSurveyAnswerOptions)
                .FirstAsync(d => d.Id == documentId, ct);

            if (doc.Status != Status.Draft && doc.Status != Status.New) throw new InvalidOperationException("Only New/Draft can be upgraded.");

            var header = await _db.SurveyHeaders
                .Include(h => h.Items)
                    .ThenInclude(i => i.CheckboxOptions)
                .FirstAsync(h => h.Id == doc.TemplateHeaderId, ct);

            if (header.StructuralVersion <= doc.StructuralVersionSnapshot) return;

            var existingItemIds = doc.Answers.Where(a => a.ItemId.HasValue).Select(a => a.ItemId!.Value).ToHashSet();

            var newItems = header.Items
                .OrderBy(i => i.SortOrder)
                .Where(i => !existingItemIds.Contains(i.Id))
                .ToList();

            foreach (var item in newItems)
            {
                doc.Answers.Add(new DocumentSurveyAnswer
                {
                    DocumentId = doc.Id,
                    ItemId = item.Id,
                    QuestionSnapshot = item.Question,
                    TypeSnapshot = item.type,
                    SortOrderSnapshot = item.SortOrder,
                    CreatedAt = DateTime.UtcNow,
                });
            }

            // for each existing answer with ItemId -> update SortOrderSnapshot = item.SortOrder

            doc.TemplateNameSnapshot = header.TemplateName;
            doc.ThemeSnapshot = header.Theme;
            doc.StructuralVersionSnapshot = header.StructuralVersion;

            await _db.SaveChangesAsync(ct);
        }
        private async Task SilentSyncSortOrderForDraftAsync(DocumentSurvey doc, CancellationToken ct = default)
        {
            if (doc.Status != Status.New && doc.Status != Status.Draft) return;
            if (doc.TemplateHeaderId <= 0) return;

            var sortMap = await _db.SurveyItems
                .AsNoTracking()
                .Where(i => i.HeaderId == doc.TemplateHeaderId)
                .Select(i => new { i.Id, i.SortOrder })
                .ToDictionaryAsync(x => x.Id, x => x.SortOrder, ct);

            var changed = false;
            foreach (var ans in doc.Answers)
            {
                if (ans.ItemId is int itemId && sortMap.TryGetValue(itemId, out var newOrder))
                {
                    if (ans.SortOrderSnapshot != newOrder)
                    {
                        ans.SortOrderSnapshot = newOrder;
                        ans.UpdatedAt = DateTime.UtcNow;
                        changed = true;
                    }
                }
            }

            if (changed)
                await _db.SaveChangesAsync(ct);
        }
        public async Task<DocumentSurveyDto> GetForRenderAsync(int documentId)
        {
            var doc = await _repo.GetDocumentWithAnswersAsync(documentId);
            if (doc == null) throw new InvalidOperationException("Document tidak ditemukan.");

            await SilentSyncSortOrderForDraftAsync(doc);

            var readOnly = !(doc.Status == Status.New || doc.Status == Status.Draft);

            var currentStructuralVersion = await _db.SurveyHeaders
                .AsNoTracking()
                .Where(h => h.Id == doc.TemplateHeaderId)
                .Select(h => h.StructuralVersion)
                .FirstOrDefaultAsync();

            var dto = new DocumentSurveyDto
            {
                Id = doc.Id,
                DocumentNo = doc.DocumentNo,
                DocumentDate = doc.DocumentDate,
                Status = (int)doc.Status,
                IsReadOnly = readOnly,

                RequesterEmployeeId = doc.RequesterEmployeeId,
                RequesterEmployeeName = doc.RequesterEmployeeName,
                RequesterPositionLevel = doc.RequesterPositionLevel,
                RequesterPositionName = doc.RequesterPositionName,
                SupervisorId = doc.SupervisorId,
                SupervisorName = doc.SupervisorName,

                TemplateCode = doc.TemplateCodeSnapshot,
                TemplateName = doc.TemplateNameSnapshot,
                Theme = doc.ThemeSnapshot,

                TemplateStructuralVersion = doc.StructuralVersionSnapshot,
                CurrentStructuralVersion = currentStructuralVersion,
                HasTemplateUpdate = currentStructuralVersion > doc.StructuralVersionSnapshot,

                Answers = doc.Answers
                .OrderBy(a => a.SortOrderSnapshot)
                .Select(a => new DocumentSurveyAnswerDto
                {
                    AnswerId = a.Id,
                    ItemId = a.ItemId ?? 0,
                    Question = a.QuestionSnapshot,
                    Type = (int)a.TypeSnapshot,
                    SortOrder = a.SortOrderSnapshot,
                    Answer = a.Answer,
                    OtherText = a.OtherText,

                    Options = (!readOnly && a.Item != null && a.Item.type == QuestionType.Checkbox)
                        ? a.Item.CheckboxOptions
                            .Select(o => new CheckboxOptionDto { Id = o.Id, Text = o.Name, IsOther = false })
                            .Concat(new[]
                            {
                        new CheckboxOptionDto { Id = -1, Text = "Lainnya", IsOther = true }
                            })
                            .ToList()
                        : new List<CheckboxOptionDto>(),

                    SelectedOptionIds = (!readOnly)
                        ? a.DocumentSurveyAnswerOptions
                            .Where(x => x.CheckboxOptionId.HasValue)
                            .Select(x => x.CheckboxOptionId!.Value)
                            .Concat(!string.IsNullOrWhiteSpace(a.OtherText) ? new[] { -1 } : Array.Empty<int>())
                            .ToList()
                        : new List<int>(),

                    SelectedOptionTexts = a.DocumentSurveyAnswerOptions
                        .Select(x => x.OptionTextSnapshot)
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .Concat(!string.IsNullOrWhiteSpace(a.OtherText) ? new[] { $"Lainnya: {a.OtherText}" } : Array.Empty<string>())
                        .ToList(),
                })
                .ToList()
            };
            return dto;
        }
        
        public async Task<List<DocumentSurveyDto>> GetListAsync()
        {
            return await _db.DocumentSurveys
                .AsNoTracking()
                .OrderByDescending(d => d.DocumentDate)
                .Select(d => new DocumentSurveyDto
                {
                    Id = d.Id,
                    DocumentNo = d.DocumentNo,
                    DocumentDate = d.DocumentDate,
                    Status = (int)d.Status,
                    IsReadOnly = d.Status != Status.Draft,

                    RequesterEmployeeId = d.RequesterEmployeeId,
                    RequesterEmployeeName = d.RequesterEmployeeName,
                    RequesterPositionLevel = d.RequesterPositionLevel,
                    RequesterPositionName = d.RequesterPositionName,
                    SupervisorId = d.SupervisorId,
                    SupervisorName = d.SupervisorName,

                    Answers = new List<DocumentSurveyAnswerDto>()
                })
                .ToListAsync();
        }
        public async Task<string> GenerateDocumentNoAsync()
        {
            var now = DateTime.Now; 
            var prefix = $"SURVEY/{now:yyMM}/";

            var lastDoc = await _db.DocumentSurveys
                .Where(d => d.DocumentNo.StartsWith(prefix))
                .OrderByDescending(d => d.DocumentNo)
                .Select(d => d.DocumentNo)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastDoc != null)
            {
                var parts = lastDoc.Split('/');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}{nextNumber:0000}";
        }

        public async Task SaveDraftAsync(DocumentSurveySaveRequestDto req)
        {
            var doc = await _repo.GetDocumentWithAnswersAsync(req.DocumentId);
            if (doc == null) throw new InvalidOperationException("Document tidak ditemukan.");
            if (doc.Status != Status.New && doc.Status != Status.Draft)
                throw new InvalidOperationException("Document sudah tidak bisa diedit.");

            var answerById = doc.Answers.ToDictionary(a => a.Id);

            foreach (var input in req.Answers)
            {
                if (!answerById.TryGetValue(input.AnswerId, out var answer))
                    throw new InvalidOperationException($"AnswerId {input.AnswerId} tidak valid.");

                if (answer.TypeSnapshot != QuestionType.Checkbox)
                {
                    answer.Answer = input.Answer;
                    answer.OtherText = null;
                    answer.UpdatedAt = DateTime.UtcNow;
                    continue;
                }

                if (answer.Item == null)
                    throw new InvalidOperationException("Template item untuk checkbox sudah hilang, draft tidak bisa diubah.");

                answer.DocumentSurveyAnswerOptions.Clear();

                var selected = (input.SelectedCheckboxOptionIds ?? new List<int>())
                    .Distinct()
                    .ToList();

                var hasOther = selected.Contains(-1);

                answer.OtherText = hasOther ? (input.OtherText ?? string.Empty) : null;
                answer.Answer = null;

                selected.Remove(-1);

                var optionMap = answer.Item.CheckboxOptions.ToDictionary(o => o.Id);

                foreach (var optId in selected)
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

            if (doc.Status == Status.New) doc.Status = Status.Draft;
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
        public async Task<List<TemplateOptionDto>> GetTemplateOptionsAsync(int positionId)
        {
            return await _db.SurveyHeaders
                .Where(h => h.PositionId == positionId)
                .AsNoTracking()
                .Select(h => new TemplateOptionDto
                {
                    Id = h.Id,
                    TemplateCode = h.TemplateCode,
                    TemplateName = h.TemplateName,
                    Theme = h.Theme,
                    ItemCount = h.Items.Count
                })
                .ToListAsync();
        }
        
        public async Task DeleteAsync(int documentId)
        {
            var doc = await _db.DocumentSurveys
                .Include(d => d.Answers)
                    .ThenInclude(a => a.DocumentSurveyAnswerOptions)
                .FirstOrDefaultAsync(d => d.Id == documentId);

            if (doc == null)
                throw new InvalidOperationException("Document tidak ditemukan.");

            // Del options
            foreach (var ans in doc.Answers)
            {
                ans.DocumentSurveyAnswerOptions.Clear();
            }

            // Del answers
            _db.DocumentSurveyAnswers.RemoveRange(doc.Answers);

            // Del dokumennya
            _db.DocumentSurveys.Remove(doc);

            await _db.SaveChangesAsync();
        }

        public async Task<DocumentSurveyPreviewDto> GetPreviewFromTemplateAsync(int templateHeaderId)
        {
            var header = await _db.SurveyHeaders
                .AsNoTracking()
                .Include(h => h.Items)
                    .ThenInclude(i => i.CheckboxOptions)
                .FirstOrDefaultAsync(h => h.Id == templateHeaderId);

            if (header == null) throw new InvalidOperationException("Survey template tidak ditemukan.");

            return new DocumentSurveyPreviewDto
            {
                TemplateHeaderId = header.Id,
                TemplateCode = header.TemplateCode,
                TemplateName = header.TemplateName,
                Theme = header.Theme,
                Answers = header.Items
                    .OrderBy(i => i.SortOrder)
                    .Select(i => new DocumentSurveyPreviewAnswerDto
                    {
                        ItemId = i.Id,
                        Question = i.Question,
                        Type = (int)i.type,
                        SortOrder = i.SortOrder,
                        Options = (i.type == QuestionType.Checkbox)
                        ? i.CheckboxOptions
                            .Select(o => new CheckboxOptionDto
                            {
                                Id = o.Id,
                                Text = o.Name,
                                IsOther = false
                            })
                            .Concat(new[]
                            {
                                new CheckboxOptionDto { Id = -1, Text = "Lainnya", IsOther = true }
                            })
                            .ToList()
                        : new List<CheckboxOptionDto>()
                    })
                    .ToList()
            };
        }
        public async Task<List<DocumentSurveyDto>> GetListForCurrentUserAsync(int userId)
        {
            var uid = userId.ToString();

            return await _db.DocumentSurveys
                .AsNoTracking()
                .Where(d =>
                    d.CreatedByUserId == userId ||
                    d.SupervisorId == uid
                )
                .OrderByDescending(d => d.DocumentDate)
                .Select(d => new DocumentSurveyDto
                {
                    Id = d.Id,
                    DocumentNo = d.DocumentNo,
                    DocumentDate = d.DocumentDate,
                    Status = (int)d.Status,
                    IsReadOnly = d.Status != Status.Draft,

                    RequesterEmployeeId = d.RequesterEmployeeId,
                    RequesterEmployeeName = d.RequesterEmployeeName,
                    RequesterPositionLevel = d.RequesterPositionLevel,
                    RequesterPositionName = d.RequesterPositionName,
                    SupervisorId = d.SupervisorId,
                    SupervisorName = d.SupervisorName,

                    Answers = new List<DocumentSurveyAnswerDto>()
                })
                .ToListAsync();
        }

        public async Task ApproveAsync(int documentId, int approverUserId)
        {
            var doc = await _repo.GetDocumentWithAnswersAsync(documentId);
            if (doc == null) throw new InvalidOperationException("Document tidak ditemukan.");
            if (doc.Status != Status.ConfirmToApprove)
                throw new InvalidOperationException("Document belum siap di-approve.");

            if (!int.TryParse(doc.RequesterEmployeeId, out var requesterUserId))
                throw new InvalidOperationException("RequesterEmployeeId tidak valid.");

            var isApprover = await _userService.IsSupervisorOfUserAsync(approverUserId, requesterUserId);
            if (!isApprover)
                throw new UnauthorizedAccessException("Anda bukan approver untuk document ini.");

            doc.Status = Status.Confirmed;
            await _repo.SaveChangesAsync();
        }

        private static int? ParseUserId(string? supervisorId)
        {
            return int.TryParse(supervisorId, out var id) ? id : null;
        }
    }
}