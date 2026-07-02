using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.UserDTOs;
public class UpdateUserDto {
    [Required(ErrorMessage = "Provide your full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress(ErrorMessage = "Provide Email address")]
    public string Email {get; set;} = string.Empty;

    [Required(ErrorMessage = "Provide Phone Number")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be 10 digits.")]
    public string PhoneNumber { get; set; } = string.Empty;


    public UpdateUserDto GetUpdatedUserDto(UpdateUserDto user){
        FullName = user.FullName;
        Email = user.Email;
        PhoneNumber = user.PhoneNumber;

        return this;
    }
}  