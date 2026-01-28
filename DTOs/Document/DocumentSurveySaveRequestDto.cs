using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.DTOs.Document
{
    public class DocumentSurveySaveRequestDto
    {
        public int DocumentId { get; set; }
        public List<DocumentSurveyAnswerSaveDto> Answers { get; set; } = new();
    }
}