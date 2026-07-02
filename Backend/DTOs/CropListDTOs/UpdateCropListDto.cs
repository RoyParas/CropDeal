using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Backend.DTOs.CropListDTOs;

public class UpdateCropListDto
{
    [Required(ErrorMessage = "Provide Price Per Kg for your crop")]
    [SwaggerSchema("The price of the crop per kg")]
    public float PricePerKg { get; set; }

    [Required(ErrorMessage = "Provide Quantity in Kgs for your crop")]
    [SwaggerSchema("The quantity of the crop in kilograms")]
    public float QuantityInKg { get; set; }

    [SwaggerSchema("Description of the crop")]
    public string Description { get; set; } = string.Empty;
}
