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
        public string Password { get; set; } = null!;
        public string PositionName { get; set; } = null!; // Raja Iblis, Petani Sawit
        public DateTime CreatedAt { get; set; }

        // FK
        public int RoleId { get; set; }
        public int PositionId { get; set; }

        // navigation
        public Role Role { get; set; } = null!;
        public Position Position { get; set; } = null!;

        // relasi supervisor
        public ICollection<UserRelation> Supervisors { get; set; } = new List<UserRelation>();
        public ICollection<UserRelation> Subordinates { get; set; } = new List<UserRelation>();
    }
}