using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.Entities.SurveyEntities
{
    public class SurveyHeader
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string TemplateCode { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public int PositionId { get; set; }
        public string Theme { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<SurveyItem> Items { get; set; } = new List<SurveyItem>();
    }
}