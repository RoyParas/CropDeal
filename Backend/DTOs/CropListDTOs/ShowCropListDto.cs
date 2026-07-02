using Backend.Models;

namespace Backend.DTOs.CropListDTOs;

public class ShowCropListDto
{
    public string nearest { get; set; } = string.Empty;

    public Guid ListingId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public float PricePerKg { get; set; }

    public float QuantityInKg { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime PostedAt { get; set; }

    public ShowCropListDto GetDto(CropListing cropList, Address dealerAddress)
    {
        ListingId = cropList.Id;
        if (cropList.Crop != null)
        {
            Name = cropList.Crop.Name;
            Type = cropList.Crop.Type;
        }
        PricePerKg = cropList.PricePerKg;
        QuantityInKg = cropList.QuantityInKg;
        ImageUrl = cropList.ImageUrl;
        Description = cropList.Description;
        PostedAt = cropList.CreatedAt;

        if (cropList.Farmer?.Address != null)
            nearest = "Same " + getNearest(cropList.Farmer.Address, dealerAddress);
        else nearest = "";

        return this;
    }

    public string getNearest(Address farmerAddress, Address dealerAddress)
    {
        if (farmerAddress.State != dealerAddress.State) return "Country";

        if (farmerAddress.District != dealerAddress.District) return "State";

        if (farmerAddress.City != dealerAddress.City) return "District";

        if (farmerAddress.ZipCode != dealerAddress.ZipCode) return "City";

        if (farmerAddress.Location != dealerAddress.Location) return "ZipCode";

        return "Locality";
    }
}