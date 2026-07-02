using Backend.DTOs.UserDTOs;
using Backend.Models;

namespace Backend.DTOs.BankDTOs;

public class ShowBankDto {
    
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public ShowUserDto? User { get; set; } 
    
    public string? BankName { get; set; }

    public string? BranchName { get; set; } 

    public string? IFSCCode { get; set; } 

    public string? AccountNumber { get; set; }

    public ShowBankDto GetDto(BankAccount account){
        
        Id = account.Id;
        UserId = account.UserId;
        User = new ShowUserDto().GetDto(account.User);
        BankName = account.BankName;
        BranchName = account.BranchName;
        IFSCCode = account.IFSCCode;
        AccountNumber = account.AccountNumber;

        return this;
    }
}