using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Backend.DTOs.CropListDTOs;

public class AddCropListDto : UpdateCropListDto
{
    [Required(ErrorMessage = "Enter the crop type")]
    [SwaggerSchema("The type of crop")]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter the crop name")]
    [SwaggerSchema("The name of the crop")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Provide Image of Crop")]
    [SwaggerSchema("An image of the crop")]
    public IFormFile? Image { get; set; }
}