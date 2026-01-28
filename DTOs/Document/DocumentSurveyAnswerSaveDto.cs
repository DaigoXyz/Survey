using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.DTOs.Document
{
    public class DocumentSurveyAnswerSaveDto
    {
        public int AnswerId { get; set; }
        public string? Answer { get; set; }
        public List<int> SelectedCheckboxOptionIds { get; set; } = new();
    }
}