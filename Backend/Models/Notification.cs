using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public string Message { get; set;} = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}