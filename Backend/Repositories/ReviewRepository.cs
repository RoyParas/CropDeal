using Backend.Data;
using Backend.DTOs.ReviewDTOs;
using Backend.Exceptions;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class ReviewRepository : IReviewRepo
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Review> AddReviewAsync(Guid dealerId, Guid transactionId, AddReviewDto addReviewDto)
        {
            var transactionFound = await _context.Transactions
                                                .Include(t => t.Listing)
                                                .FirstOrDefaultAsync(t => t.Id == transactionId && t.DealerId == dealerId && t.TransactionStatus == "Completed");
            if (transactionFound == null) throw new NotFoundException("Transaction Not Found. So, you cannot add the review");

            if (await _context.Reviews.AnyAsync(r => r.TransactionId == transactionFound.Id))
                throw new ExistException("Review already exist");

            if (transactionFound.Listing == null) throw new InvalidOperationException("Transaction does not have an associated listing.");

            var farmerId = transactionFound.Listing.FarmerId;

            Review review = new Review
            {
                DealerId = dealerId,
                FarmerId = farmerId,
                TransactionId = transactionId,
                Rating = addReviewDto.Rating,
                Comment = addReviewDto.Comment
            };

            await _context.Reviews.AddAsync(review);
            if (await _context.SaveChangesAsync() > 0)
            {
                await UpdateAverageRating(addReviewDto.Rating, farmerId);
                return review;
            }

            else throw new UnableException("Unable to add review from the dealer");
        }

        public async Task<List<Review>> GetReviewsByFarmerAsync(Guid farmerId)
        {
            return await _context.Reviews.Where(r => r.FarmerId == farmerId).ToListAsync();
        }

        public async Task<Review> GetReviewByIdAsync(Guid transactionId)
        {
            return await _context.Reviews.Where(r => r.TransactionId == transactionId).FirstOrDefaultAsync();
        }

        private async Task<bool> UpdateAverageRating(float rating, Guid farmerId)
        {
            List<Review> reviews = await _context.Reviews.Where(r => r.FarmerId == farmerId).ToListAsync();
            int reviewCount = reviews.Count;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == farmerId);
            if (user == null) throw new NotFoundException("Farmer not found to update review");

            user.AverageRating = ((user.AverageRating * (reviewCount - 1)) + rating) / reviewCount;

            return await _context.SaveChangesAsync() > 0;
        }
    }
}