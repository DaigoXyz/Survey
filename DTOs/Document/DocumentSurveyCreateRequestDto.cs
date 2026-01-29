using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.DTOs.Document
{
    public class DocumentSurveyCreateRequestDto
    {
        public string DocumentNo { get; set; } = string.Empty;
        public DateTime DocumentDate { get; set; }

        public int TemplateHeaderId { get; set; }

        public int CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;

        public string RequesterEmployeeId { get; set; } = string.Empty;
        public string RequesterEmployeeName { get; set; } = string.Empty;
        public string RequesterPositionLevel { get; set; } = string.Empty;
        public string RequesterPositionName { get; set; } = string.Empty;
        public string SupervisorId { get; set; } = string.Empty;
        public string SupervisorName { get; set; } = string.Empty;
    }
}