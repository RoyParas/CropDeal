
namespace Backend.DTOs
{
    public class ReceiptDto
    {
        public Guid TransactionNumber { get; set; }

        // Dealer Details
        public string DealerName { get; set; } = string.Empty;
        public string DealerEmail { get; set; } = string.Empty;
        public string DealerPhone { get; set; } = string.Empty;

        // Farmer Details
        public string FarmerName { get; set; } = string.Empty;
        public string FarmerEmail { get; set; } = string.Empty;
        public string FarmerPhone { get; set; } = string.Empty;

        // Crop Details
        public string CropName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Pricing & Transaction Info
        public float PricePerKg { get; set; }
        public float QuantityInKg { get; set; }
        public float TotalPrice { get; set; }

        public float Discount { get; set; }
        public float AmountPaid { get; set; }

        // Metadata
        public DateTime CreatedAt { get; set; } 
    }
}
