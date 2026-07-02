using System.ComponentModel.DataAnnotations;

namespace Backend.Models;
public class Subscription
{
    [Key]
    public Guid Id { get; set; }

    public Guid DealerId { get; set; }
    public User? Dealer { get; set; }

    public Guid CropId { get; set; }
    public Crop? Crop { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}