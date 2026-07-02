using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.BankDTOs;
public class AddBankDto {
    [Required(ErrorMessage = "Provide Bank Name")]
    public string BankName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Provide your Branch Name")]
    public string BranchName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Provide IFSC Code")]    
    public string IFSCCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Provide your Account Number")]
    public string AccountNumber { get; set; } = string.Empty;
}