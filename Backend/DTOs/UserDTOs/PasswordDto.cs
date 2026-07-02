using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.UserDTOs;
public class PasswordDto {  
    [Required(ErrorMessage = "Enter Old Password")]
    public string OldPassword {get; set;} = string.Empty;

    [Required(ErrorMessage = "Enter new Password")]
    public string NewPassword {get; set;} = string.Empty;

    [Required(ErrorMessage = "Re-Enter Password")]
    [Compare("NewPassword", ErrorMessage = "The Password does not match")]
    public string ConfirmNewPassword{get; set;} = string.Empty;
}