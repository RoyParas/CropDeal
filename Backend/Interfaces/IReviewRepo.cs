
using Backend.DTOs.ReviewDTOs;
using Backend.Models;

namespace Backend.Interfaces
{
    public interface IReviewRepo
    {
        Task<Review> AddReviewAsync(Guid dealerId, Guid transactionId, AddReviewDto addReviewDto);

        Task<Review> GetReviewByIdAsync(Guid transactionId);

        Task<List<Review>> GetReviewsByFarmerAsync(Guid farmerId);
    }
}