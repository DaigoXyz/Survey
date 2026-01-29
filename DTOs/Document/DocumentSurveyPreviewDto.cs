using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.DTOs.Document
{
    public class DocumentSurveyPreviewDto
    {
        public int TemplateHeaderId { get; set; }
        public string TemplateCode { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty;

        public List<DocumentSurveyPreviewAnswerDto> Answers { get; set; } = new();
    }

    public class DocumentSurveyPreviewAnswerDto
    {
        public int ItemId { get; set; }
        public string Question { get; set; } = string.Empty;
        public int Type { get; set; }
        public int SortOrder { get; set; }

        public string? Answer { get; set; }
        public List<CheckboxOptionDto> Options { get; set; } = new();
        public List<int> SelectedOptionIds { get; set; } = new();
    }
}