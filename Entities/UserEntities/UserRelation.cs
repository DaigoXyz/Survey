using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Survey.Entities
{
    [Index(nameof(UserId), IsUnique = true)]
    public class UserRelation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SupervisorId { get; set; }
        public User Supervisor { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
