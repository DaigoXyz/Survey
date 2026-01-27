using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.Entities.DocumentEntities;

namespace Survey.Entities.SurveyEntities
{
    public class CheckboxOption
    {
        public int Id { get; set; }

        public int ItemId { get; set; }
        public string Name { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // navigation
        public SurveyItem Item { get; set; } = null!;

        public ICollection<DocumentSurveyAnswerOption> DocumentSurveyAnswerOptions { get; set; }
            = new List<DocumentSurveyAnswerOption>();
    }
}