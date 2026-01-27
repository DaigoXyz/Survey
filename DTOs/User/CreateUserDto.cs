using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.DTOs.User
{
    public class CreateUserDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int PositionId { get; set; }
        public string PositionName { get; set; } = null!;
        public int SupervisorId { get; set; }
    }
}