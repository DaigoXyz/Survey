using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.Entities.SurveyEntities;

namespace Survey.Entities.DocumentEntities
{
    public class DocumentSurveyAnswer
    {
        public int Id { get; set; }

        public int DocumentId { get; set; }
        public DocumentSurvey Document { get; set; } = null!;
        public int? ItemId { get; set; }
        public SurveyItem? Item { get; set; }

        public string QuestionSnapshot { get; set; } = string.Empty;
        public QuestionType TypeSnapshot { get; set; }
        public int SortOrderSnapshot { get; set; }

        public string? Answer { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<DocumentSurveyAnswerOption> DocumentSurveyAnswerOptions { get; set; }
            = new List<DocumentSurveyAnswerOption>();
    }
}