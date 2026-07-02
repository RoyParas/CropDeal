using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Backend.DTOs.CropListDTOs;

public class ImageToUpdate
{
    [Required(ErrorMessage = "Provide Image of Crop")]
    [SwaggerSchema("An image of the crop")]
    public IFormFile? Image { get; set; }
}