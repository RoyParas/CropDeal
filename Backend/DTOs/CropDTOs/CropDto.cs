using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.CropDTOs;

public class CropDto {
    [Required(ErrorMessage = "Provide Crop Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Provide Type")]
    public string Type { get; set; } = string.Empty;
}