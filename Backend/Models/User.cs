using Microsoft.AspNetCore.Identity;

namespace Backend.Models;
public class User : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;

    public override string? PhoneNumber { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public Guid? AddressId { get; set; }
    public Address? Address { get; set; }

    public Guid? BankAccountId { get; set; }
    public BankAccount? BankAccount { get; set; }

    public float? AverageRating { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}