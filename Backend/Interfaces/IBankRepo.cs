using Backend.DTOs.BankDTOs;
using Backend.Models;

namespace Backend.Interfaces
{
    public interface IBankRepo
    {
        // List<BankAccount> GetBankAccounts();

        // Task<BankAccount> GetUserBankAccountAsync(Guid userId);

        Task<BankAccount> AddAccountAsync(Guid userId, AddBankDto bankDtoObj);  

        Task<bool> DeleteAccountAsync(Guid userId);
    }
}