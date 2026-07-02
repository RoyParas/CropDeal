using Backend.Models;

namespace Backend.DTOs.UserDTOs;

public class ShowUserDto{
    public string FullName{get; set;} = string.Empty;

    public string? Email{get; set;} = string.Empty;
    
    public string? PhoneNumber{get; set;} = string.Empty;

    public string Role { get; set; } = string.Empty;

    public float? AverageRating { get; set; }

    public bool IsActive { get; set; }

    public ShowUserDto GetDto(User user){
        FullName = user.FullName;
        Email = user.Email;
        PhoneNumber = user.PhoneNumber;
        Role = user.Role;
        AverageRating = user.AverageRating;
        IsActive = user.IsActive;

        return this;
    }
}