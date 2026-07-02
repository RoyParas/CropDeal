using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Review
{
    [Key]
    public Guid Id { get; set; }

    public Guid DealerId { get; set; }
    public User? Dealer { get; set; }

    public Guid FarmerId { get; set; }
    public User? Farmer { get; set; }

    public Guid TransactionId { get; set; }
    public Transaction? Transaction { get; set; }

    public int Rating { get; set; } 

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    // public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
