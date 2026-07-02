using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.AddressDTOs;
public class AddressDto {
    [Required(ErrorMessage = "Provide Location")]
    public string Location { get; set; } = string.Empty;

    [Required(ErrorMessage = "Provide City")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Provide District")]
    public string District { get; set; } = string.Empty;

    [Required(ErrorMessage = "Provide State")]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "Provide ZipCode")]
    public string ZipCode { get; set; } = string.Empty;
}