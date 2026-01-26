using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Survey.Entities
{
    public class Position
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Created_at { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}