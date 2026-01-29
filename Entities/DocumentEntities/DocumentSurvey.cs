using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Survey.Entities.DocumentEntities
{
    public class DocumentSurvey
    {
        public int Id { get; set; }

        [MaxLength(30)]
        public string DocumentNo { get; set; } = string.Empty;

        public DateTime DocumentDate { get; set; }
        public Status Status { get; set; }

        public int CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string RequesterEmployeeId { get; set; } = string.Empty;

        public string RequesterEmployeeName { get; set; } = string.Empty;
        public string RequesterPositionLevel { get; set; } = string.Empty;
        public string RequesterPositionName { get; set; } = string.Empty;
        public string SupervisorId { get; set; } = string.Empty;

        public string SupervisorName { get; set; } = string.Empty;

        public string TemplateCodeSnapshot { get; set; } = string.Empty;
        public string TemplateNameSnapshot { get; set; } = string.Empty;

        public string ThemeSnapshot { get; set; } = string.Empty;

        public ICollection<DocumentSurveyAnswer> Answers { get; set; } = new List<DocumentSurveyAnswer>();
    }

    public enum Status
    {
        New = 0,
        Draft = 1,
        [Display(Name = "Confirm To Approve")]
        ConfirmToApprove = 2,
        Confirmed = 3
    }
}
