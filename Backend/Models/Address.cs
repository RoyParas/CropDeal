using System.ComponentModel.DataAnnotations;

namespace Backend.Models;
public class Address
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public string Location { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string District { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;

    // public DateTime CreatedAt{get; set;} = DateTime.Now;

    // public DateTime UpdatedAt{get; set;} = DateTime.Now;
}
