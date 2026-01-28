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

        public List<DocumentSurveyAnswerDto> Answers { get; set; } = new();
    }

    public class DocumentSurveyAnswerDto
    {
        public int AnswerId { get; set; }

        public string Question { get; set; } = string.Empty;
        public int Type { get; set; }  
        public int SortOrder { get; set; }

        public string? Answer { get; set; }

        public List<CheckboxOptionDto> Options { get; set; } = new();
        public List<int> SelectedOptionIds { get; set; } = new();
        public List<string> SelectedOptionTexts { get; set; } = new(); 
    }

    public class CheckboxOptionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}