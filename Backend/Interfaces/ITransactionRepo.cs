
using Backend.DTOs.TransactionDTOs;
using Backend.Models;

namespace Backend.Interfaces
{
    public interface ITransactionRepo
    {
        Task<List<Transaction>> GetAllTransactionsAync();

        Task<Transaction> GetTransactionByIdAsync(Guid transactionId);

        Task<List<Transaction>> GetTransactionsByDealerIdAsync(Guid dealerId);

        Task<List<Transaction>> GetTransactionsByFarmerIdAsync(Guid farmerId);

        Task<List<Transaction>> GetTransactionsByUserIdAsync(Guid userId);
    
        Task<bool> InitiateTransactionAsync(TransactionDto transaction, Guid dealerId, Guid cropListingId);

        Task<bool> AcceptTransactionAsync(Guid transactionId);

        Task<bool> RejectTransactionAsync(Guid transactionId, string role);

        Task<bool> MakePaymentAsync(Guid transactionId);
    }
}