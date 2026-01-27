using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.DTOs.Survey;

namespace Survey.DTOs.Survey
{
    public record SurveyHeaderCreateDto(string TemplateName, int PositionId, string Theme);
    public record SurveyHeaderUpdateDto(string TemplateName, int PositionId, string Theme);

    public record SurveyHeaderListDto(int Id, string TemplateCode, string TemplateName, int PositionId, string Theme);

    public record SurveyHeaderDetailDto(
        int Id,
        string TemplateCode,
        string TemplateName,
        int PositionId,
        string Theme,
        List<SurveyItemDto> Items
    );
}