using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.DTOs.Document
{
    public class RequesterProfileDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;

        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string PositionLevel { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;

        public string SupervisorId { get; set; } = string.Empty;
        public string SupervisorName { get; set; } = string.Empty;
    }
}