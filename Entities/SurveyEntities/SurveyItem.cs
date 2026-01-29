using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.Entities.SurveyEntities
{
    public class SurveyItem
    {
        public int Id { get; set; }
        public int HeaderId { get; set; }
        public string Question { get; set; } = string.Empty;
        public QuestionType type { get; set; }
        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public SurveyHeader Header { get; set; } = null!;
        public ICollection<CheckboxOption> CheckboxOptions { get; set; } = new List<CheckboxOption>();
    }

    public enum QuestionType
    { 
        Textbox = 1,
        Checkbox = 2,
        TextArea = 3,
        Number = 4
    }

}