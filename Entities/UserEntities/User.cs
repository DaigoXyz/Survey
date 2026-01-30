using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string PositionName { get; set; } = null!; 
        public DateTime CreatedAt { get; set; }

        public int RoleId { get; set; }
        public int PositionId { get; set; }

        public Role Role { get; set; } = null!;
        public Position Position { get; set; } = null!;

        public ICollection<UserRelation> Supervisors { get; set; } = new List<UserRelation>();
        public ICollection<UserRelation> Subordinates { get; set; } = new List<UserRelation>();
    }
}