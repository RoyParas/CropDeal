using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.UserDTOs;
public class SignInDto
{    
    [EmailAddress]
    [Required(ErrorMessage ="Please provide email address")]
    public string Email{get; set;} = string.Empty;

    [Required(ErrorMessage = "Please Provide Password")]
    public string Password{get; set;} = string.Empty;
}