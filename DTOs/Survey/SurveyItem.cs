using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.Entities.SurveyEntities;

namespace Survey.DTOs.Survey
{
    public record CheckboxOptionDto(int Id, string Name);

    public record SurveyItemDto(
        int Id,
        int HeaderId,
        string Question,
        QuestionType Type,
        int SortOrder,
        List<CheckboxOptionDto> CheckboxOptions
    );
    public record SurveyItemCreateDto(string Question, QuestionType Type, List<string>? CheckboxOptions);
    public record SurveyItemUpdateDto(string Question, QuestionType Type, List<string>? CheckboxOptions);

    public record ReorderItemDto(int ItemId, string Direction);
}