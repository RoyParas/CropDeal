using System.ComponentModel.DataAnnotations;
using Backend.Enums;

namespace Backend.Models;

public class Transaction
{

    [Key]
    public Guid Id { get; set; }

    public Guid DealerId { get; set; }
    public User? Dealer { get; set; }

    public Guid ListingId { get; set; }
    public CropListing? Listing { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Quantity must be a positive value.")]
    public float QuantityInKg { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
    public float FinalPricePerKg { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Total price must be a positive value.")]
    public float TotalPrice { get; set; }

    public string TransactionStatus { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public void UpdateTotalPrice() {
        TotalPrice = QuantityInKg * FinalPricePerKg;
    }
}
