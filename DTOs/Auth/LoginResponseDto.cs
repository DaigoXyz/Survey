using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.DTOs.Auth
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}