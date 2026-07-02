using OfficeOpenXml;
using OfficeOpenXml.Style;
using Backend.Data;
using Backend.DTOs.TransactionDTOs;
using Backend.Exceptions;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class TransactionRepository : ITransactionRepo
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> GetAllTransactionsAync()
        {
            return await _context.Transactions.OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        public async Task<Transaction> GetTransactionByIdAsync(Guid transactionId)
        {
            var transaction = await _context.Transactions
                                            .Include(t => t.Dealer)
                                            .Include(t => t.Listing)
                                                .ThenInclude(l => l.Crop)
                                            .Include(t => t.Listing)
                                                .ThenInclude(l => l.Farmer)
                                            .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null) throw new NotFoundException("Transaction Not Found");

            else return transaction;
        }

        public async Task<List<Transaction>> GetTransactionsByDealerIdAsync(Guid dealerId)
        {
            return await _context.Transactions.OrderByDescending(t => t.CreatedAt).Where(t => t.DealerId == dealerId).ToListAsync();
        }

        public async Task<List<Transaction>> GetTransactionsByFarmerIdAsync(Guid farmerId)
        {
            return await _context.Transactions
                                .OrderByDescending(t => t.CreatedAt)
                                .Include(t => t.Listing)
                                .Where(t => t.Listing != null && t.Listing.FarmerId == farmerId)
                                .ToListAsync();
        }

        public async Task<List<Transaction>> GetTransactionsByUserIdAsync(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) throw new NotFoundException("User not found");
            string role = user.Role;
            List<Transaction> transactions = [];

            if (role == "Farmer")
            {
                transactions = await _context.Transactions
                                        .Include(t => t.Dealer)
                                        .Include(t => t.Listing)
                                            .ThenInclude(l => l.Crop)
                                        .Where(t => t.Listing.FarmerId == userId)
                                        .ToListAsync();
            }

            else if (role == "Dealer")
            {
                transactions = await _context.Transactions
                                        .Include(t => t.Listing)
                                            .ThenInclude(l => l.Farmer)
                                        .Include(t => t.Listing)
                                            .ThenInclude(l => l.Crop)
                                        .Where(t => t.DealerId == userId)
                                        .ToListAsync();
            }
            return transactions;
        }
        
        public async Task<bool> InitiateTransactionAsync(TransactionDto transactionDtoObj, Guid dealerId, Guid cropListingId)
        {
            var cropFound = await _context.CropListings.FirstOrDefaultAsync(cl => cl.Id == cropListingId);

            if (cropFound == null) throw new NotFoundException("listed crop not Found");

            if (!await _context.BankAccounts.AnyAsync(b => b.UserId == dealerId))
                throw new UnableException("You have not added Bank Account. Add Bank account to initiate Transaction");

            if (cropFound.QuantityInKg < transactionDtoObj.QuantityInKg)
                throw new UnableException("Unable to initiate transaction as your mentioned quantity exceeds the listed crop quantity");

            Transaction transaction = new Transaction
            {
                DealerId = dealerId,
                ListingId = cropListingId,
                QuantityInKg = transactionDtoObj.QuantityInKg,
                FinalPricePerKg = transactionDtoObj.PricePerKg
            };

            transaction.UpdateTotalPrice();
            await _context.Transactions.AddAsync(transaction);
            cropFound.QuantityInKg -= transactionDtoObj.QuantityInKg;

            if (cropFound.QuantityInKg == 0)
            {
                cropFound.AvailabilityStatus = false;
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AcceptTransactionAsync(Guid transactionId)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null) throw new NotFoundException("Transaction not found");

            if (transaction.TransactionStatus != "Pending") throw new UnableException("Cannot accept transaction as it is not in pending state");

            transaction.TransactionStatus = "Accepted";
            transaction.UpdatedAt = DateTime.Now;

            if (await _context.SaveChangesAsync() > 0) return true;
            return false;
        }

        public async Task<bool> RejectTransactionAsync(Guid transactionId, string role)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);

            if(transaction == null) throw new NotFoundException("Transaction not found");

            var listedCropId = await _context.Transactions.Where(t => t.Id == transactionId).Select(t => t.ListingId).FirstAsync();

            var cropFound = await _context.CropListings.FirstOrDefaultAsync(cl => cl.Id == listedCropId);

            if(cropFound == null) throw new NotFoundException("listed crop not Found");

            if ((transaction.TransactionStatus == "Pending" && role == "Farmer") || (transaction.TransactionStatus == "Accepted" && role == "Dealer"))
            {
                transaction.TransactionStatus = "Rejected";
                cropFound.QuantityInKg += transaction.QuantityInKg;
                transaction.UpdatedAt = DateTime.Now;
            }
            else throw new UnableException("Cannot reject transaction due to state or role difference");

            if(cropFound.AvailabilityStatus == false) cropFound.AvailabilityStatus = true;

            if(await _context.SaveChangesAsync() > 0) return true;
            return false;
        }

        public async Task<bool> MakePaymentAsync(Guid transactionId)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null) throw new NotFoundException("Transaction not found");

            if (transaction.TransactionStatus != "Accepted") throw new UnableException("Cannot make payment as transaction is not accepted by farmer");

            transaction.TransactionStatus = "Completed";
            transaction.UpdatedAt = DateTime.Now;

            if (await _context.SaveChangesAsync() > 0) return true;
            return false;
        }
    }
}