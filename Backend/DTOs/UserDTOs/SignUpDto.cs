using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.UserDTOs;
public class SignUpDto
{    
    [Required(ErrorMessage ="Please provide Full Name")]
    public string FullName{get; set;} = string.Empty;

    [EmailAddress]
    [Required(ErrorMessage ="Please provide email address")]
    public string Email{get; set;} = string.Empty;
    
    [Required(ErrorMessage ="Please provide Phone Number")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be 10 digits.")]
    public string PhoneNumber{get; set;} = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    public string Role { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please Provide Password")]
    public string Password{get; set;} = string.Empty;
    
    [Required(ErrorMessage = "Re-Enter Password")]
    // [Compare("Password", ErrorMessage = "The Password does not match")]
    public string ConfirmPassword{get; set;} = string.Empty;
}