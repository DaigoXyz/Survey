using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.DTOs.Document
{
    public class DocumentSurveyDto
    {
        public int Id { get; set; }
        public string DocumentNo { get; set; } = string.Empty;
        public DateTime DocumentDate { get; set; }
        public int Status { get; set; }  
        public bool IsReadOnly { get; set; }
        public string TemplateCode { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty;
        public string RequesterEmployeeId { get; set; } = string.Empty;
        public string RequesterEmployeeName { get; set; } = string.Empty;
        public string RequesterPositionLevel { get; set; } = string.Empty;
        public string RequesterPositionName { get; set; } = string.Empty;
        public string SupervisorId { get; set; } = string.Empty;
        public string SupervisorName { get; set; } = string.Empty;
        public bool HasTemplateUpdate { get; set; }
        public int TemplateStructuralVersion { get; set; }
        public int CurrentStructuralVersion { get; set; }
        public List<DocumentSurveyAnswerDto> Answers { get; set; } = new();
    }

    public class DocumentSurveyAnswerDto
    {
        public int AnswerId { get; set; }
        public int ItemId { get; set; }
        public string Question { get; set; } = string.Empty;
        public int Type { get; set; }  
        public int SortOrder { get; set; }
        public string? Answer { get; set; }
        public string? OtherText { get; set;}
        public List<CheckboxOptionDto> Options { get; set; } = new();
        public List<int> SelectedOptionIds { get; set; } = new();
        public List<string> SelectedOptionTexts { get; set; } = new(); 
    }

    public class CheckboxOptionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsOther { get; set; }
    }
}