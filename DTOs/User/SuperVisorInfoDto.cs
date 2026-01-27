using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.DTOs.User
{
    public class SuperVisorInfoDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public int PositionId { get; set; }
        public string PositionName { get; set; } = string.Empty;
    }
}