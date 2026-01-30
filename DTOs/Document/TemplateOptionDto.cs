using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.DTOs.Document
{
    public class TemplateOptionDto
    {
        public int Id { get; set; }
        public string TemplateCode { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public int PositionId { get; set; } 
    }
}