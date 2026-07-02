using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class PasswordResetToken
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid Token { get; set; }
        public DateTime Expiry { get; set; } = DateTime.UtcNow.AddHours(1);
        public bool IsUsed { get; set; } = false;

        public User? User { get; set; }
    }
}