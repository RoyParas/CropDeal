using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class CropListing
{
    [Key]
    public Guid Id { get; set; }

    public Guid FarmerId { get; set; }
    public User? Farmer { get; set; }

    public Guid CropId { get; set; }
    public Crop? Crop { get; set; }

    public float PricePerKg { get; set; }

    public float QuantityInKg { get; set; }

    public bool AvailabilityStatus { get; set; } = true;

    public string ImageUrl { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty; 

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}