using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class BankAccount
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; } 
    
    public string BankName { get; set; } = string.Empty;

    public string BranchName { get; set; } = string.Empty;
 
    public string IFSCCode { get; set; } = string.Empty;

    public string AccountNumber { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
} 