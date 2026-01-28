using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Survey.Entities.DocumentEntities
{
    public class DocumentSurvey
    {
        public int Id { get; set; }
        public string DocumentNo { get; set; } = string.Empty;
        public DateTime DocumentDate { get; set; }
        public Status Status { get; set; }

        public ICollection<DocumentSurveyAnswer> Answers { get; set; } = new List<DocumentSurveyAnswer>();
    }

    public enum Status
    {
        Draft = 1,

        [Display(Name = "Confirm To Approve")]
        ConfirmToApprove = 2,
        Confirmed = 3
    }
}