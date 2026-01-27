using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.DTOs.User
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public int PositionId { get; set; }
        public string PositionName { get; set; } = "";
        public int SupervisorId { get; set; }
    }
}