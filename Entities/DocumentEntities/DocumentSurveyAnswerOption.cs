using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survey.Entities.SurveyEntities;

namespace Survey.Entities.DocumentEntities
{
    public class DocumentSurveyAnswerOption
    {
        public int AnswerId { get; set; }
        public DocumentSurveyAnswer Answer { get; set; } = null!;

        public int? CheckboxOptionId { get; set; }
        public CheckboxOption? CheckboxOption { get; set; }

        public string OptionTextSnapshot { get; set; } = string.Empty;
    }

}