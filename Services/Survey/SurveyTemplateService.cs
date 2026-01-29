using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Survey.Data;
using Survey.Entities.SurveyEntities;
using Survey.DTOs.Survey;
using Survey.Repositories.IRepositories.ISurveyRepository;
using Survey.Helpers;

namespace Survey.Services.Survey
{
    public class SurveyTemplateService : ISurveyTemplateService
    {
        private readonly AppDbContext _db;
        private readonly ISurveyHeaderRepository _headerRepo;
        private readonly ISurveyItemRepository _itemRepo;
        private readonly IUnitOfWork _uow;

        public SurveyTemplateService(
            AppDbContext db,
            ISurveyHeaderRepository headerRepo,
            ISurveyItemRepository itemRepo,
            IUnitOfWork uow)
        {
            _db = db;
            _headerRepo = headerRepo;
            _itemRepo = itemRepo;
            _uow = uow;
        }

        public async Task<SurveyHeaderDetailDto> CreateHeaderAsync(SurveyHeaderCreateDto dto, CancellationToken ct = default)
        {
            const int maxRetry = 3;

            for (int attempt = 1; attempt <= maxRetry; attempt++)
            {
                var now = DateTime.UtcNow;
                var yearMonth = now.ToString("yyyyMM");

                var lastCode = await _headerRepo.GetLastTemplateCodeForYearMonthAsync(yearMonth, ct);

                var nextNumber = 1;
                if (!string.IsNullOrWhiteSpace(lastCode))
                {
                    var lastPart = lastCode.Split('/').Last(); // "003"
                    if (int.TryParse(lastPart, out var n)) nextNumber = n + 1;
                }

                var templateCode = $"TEMPLATE/{yearMonth}/{nextNumber:D3}";

                var header = new SurveyHeader
                {
                    TemplateCode = templateCode,
                    TemplateName = dto.TemplateName,
                    PositionId = dto.PositionId,
                    Theme = dto.Theme,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                await _headerRepo.AddAsync(header, ct);

                try
                {
                    await _uow.SaveChangesAsync(ct);
                    return await GetDetailAsync(header.Id, ct);
                }
                catch (DbUpdateException)
                {
                    _db.Entry(header).State = EntityState.Detached;

                    if (attempt == maxRetry) throw;
                }
            }

            throw new Exception("Failed to generate template code after retries.");
        }

        public async Task<List<SurveyHeaderListDto>> ListAsync(CancellationToken ct = default)
        {
            var headers = await _headerRepo.ListAsync(ct);
            return headers.Select(x => new SurveyHeaderListDto(
                x.Id, x.TemplateCode, x.TemplateName, x.PositionId, x.Theme
            )).ToList();
        }

        public async Task<SurveyHeaderDetailDto> GetDetailAsync(int headerId, CancellationToken ct = default)
        {
            var header = await _headerRepo.GetByIdAsync(headerId, includeChildren: true, ct);
            if (header is null) throw new KeyNotFoundException("Survey header not found");

            return new SurveyHeaderDetailDto(
                header.Id,
                header.TemplateCode,
                header.TemplateName,
                header.PositionId,
                header.Theme,
                header.Items.OrderBy(i => i.SortOrder).Select(i => new SurveyItemDto(
                    i.Id,
                    i.HeaderId,
                    i.Question,
                    i.type,
                    i.SortOrder,
                    i.CheckboxOptions.Select(o => new CheckboxOptionDto(o.Id, o.Name)).ToList()
                )).ToList()
            );
        }

        public async Task UpdateHeaderAsync(int headerId, SurveyHeaderUpdateDto dto, CancellationToken ct = default)
        {
            var header = await _headerRepo.GetByIdAsync(headerId, false, ct);
            if (header is null) throw new KeyNotFoundException("Survey header not found");

            header.TemplateName = dto.TemplateName;
            header.PositionId = dto.PositionId;
            header.Theme = dto.Theme;
            header.UpdatedAt = DateTime.UtcNow;

            _headerRepo.Update(header);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteHeaderAsync(int headerId, CancellationToken ct = default)
        {
            var header = await _headerRepo.GetByIdAsync(headerId, false, ct);
            if (header is null) return;

            _headerRepo.Remove(header);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<SurveyItemDto> AddItemAsync(int headerId, SurveyItemCreateDto dto, CancellationToken ct = default)
        {
            var header = await _headerRepo.GetByIdAsync(headerId, false, ct);
            if (header is null) throw new KeyNotFoundException("Survey header not found");

            var now = DateTime.UtcNow;
            var sortOrder = await _itemRepo.GetNextSortOrderAsync(headerId, ct);

            var item = new SurveyItem
            {
                HeaderId = headerId,
                Question = dto.Question,
                type = dto.Type,
                SortOrder = sortOrder,
                CreatedAt = now,
                UpdatedAt = now
            };

            if (dto.Type == QuestionType.Checkbox)
            {
                var options = (dto.CheckboxOptions ?? new List<string>())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => new CheckboxOption { Name = x.Trim(), CreatedAt = now })
                    .ToList();

                item.CheckboxOptions = options;
            }

            _db.SurveyItems.Add(item);
            await _uow.SaveChangesAsync(ct);

            var loaded = await _itemRepo.GetByIdAsync(item.Id, includeOptions: true, ct);
            if (loaded is null) throw new Exception("Failed to load created item");

            return new SurveyItemDto(
                loaded.Id,
                loaded.HeaderId,
                loaded.Question,
                loaded.type,
                loaded.SortOrder,
                loaded.CheckboxOptions.Select(o => new CheckboxOptionDto(o.Id, o.Name)).ToList()
            );
        }

        public async Task UpdateItemAsync(int itemId, SurveyItemUpdateDto dto, CancellationToken ct = default)
        {
            var item = await _itemRepo.GetByIdAsync(itemId, includeOptions: true, ct);
            if (item is null) throw new KeyNotFoundException("Survey item not found");

            var now = DateTime.UtcNow;
            item.Question = dto.Question;
            item.type = dto.Type;
            item.UpdatedAt = now;

            if (dto.Type != QuestionType.Checkbox)
            {
                if (item.CheckboxOptions.Any())
                    _db.CheckboxOptions.RemoveRange(item.CheckboxOptions);
            }
            else
            {
                _db.CheckboxOptions.RemoveRange(item.CheckboxOptions);

                var names = (dto.CheckboxOptions ?? new List<string>())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .Distinct()
                    .ToList();

                item.CheckboxOptions = names.Select(n => new CheckboxOption
                {
                    Name = n,
                    CreatedAt = now
                }).ToList();
            }

            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteItemAsync(int itemId, CancellationToken ct = default)
        {
            var item = await _itemRepo.GetByIdAsync(itemId, false, ct);
            if (item is null) return;

            _db.SurveyItems.Remove(item);
            await _uow.SaveChangesAsync(ct);

            await NormalizeSortOrderAsync(item.HeaderId, ct);
        }

        public async Task ReorderItemAsync(int headerId, ReorderItemDto dto, CancellationToken ct = default)
        {
            var items = await _db.SurveyItems
                .Where(x => x.HeaderId == headerId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync(ct);

            var idx = items.FindIndex(x => x.Id == dto.ItemId);
            if (idx < 0) throw new KeyNotFoundException("Item not found");

            var dir = (dto.Direction ?? "").Trim().ToUpperInvariant();
            var swapWith = dir == "UP" ? idx - 1 : dir == "DOWN" ? idx + 1 : -1;
            if (swapWith < 0 || swapWith >= items.Count) return;

            var a = items[idx];
            var b = items[swapWith];

            var oldA = a.SortOrder;
            var oldB = b.SortOrder;

            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            a.SortOrder = 0;
            await _db.SaveChangesAsync(ct);

            b.SortOrder = oldA;
            a.SortOrder = oldB;
            await _db.SaveChangesAsync(ct);

            await tx.CommitAsync(ct);
        }

        private async Task NormalizeSortOrderAsync(int headerId, CancellationToken ct)
        {
            var items = await _db.SurveyItems
                .Where(x => x.HeaderId == headerId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync(ct);

            for (int i = 0; i < items.Count; i++)
                items[i].SortOrder = i + 1;

            await _uow.SaveChangesAsync(ct);
        }
    }
}
