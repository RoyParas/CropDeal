using System.Security.Claims;
using Backend.DTOs.ReviewDTOs;
using Backend.Exceptions;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepo _reviewRepo;

        public ReviewController(IReviewRepo reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }

        private static Guid ValidateIdString(string? idString)
        {
            if (string.IsNullOrEmpty(idString) || !Guid.TryParse(idString, out Guid id))
            {
                throw new InvalidException($"Invalid `{idString}` id string");
            }
            return id;
        }

        [Authorize(Roles = "Dealer")]
        [HttpPost("addReview")]
        public async Task<IActionResult> AddReview([FromQuery] string transactionIdString, AddReviewDto addReviewDto)
        {
            Guid dealerId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            Guid transactionId = ValidateIdString(transactionIdString);

            var addedReview = await _reviewRepo.AddReviewAsync(dealerId, transactionId, addReviewDto);

            if (addedReview == null) throw new UnableException("Unable to add review");

            return Ok(new { message = "Review added successfully", addedReview });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getFarmerReviews")]
        public async Task<IActionResult> GetFarmerReview([FromQuery] string farmerIdString)
        {
            Guid farmerId = ValidateIdString(farmerIdString);

            var farmerReviews = await _reviewRepo.GetReviewsByFarmerAsync(farmerId);

            return Ok(farmerReviews);
        }

        [Authorize(Roles = "Farmer")]
        [HttpGet("myReviews")]
        public async Task<IActionResult> GetMyReviews()
        {
            Guid farmerId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var myReviews = await _reviewRepo.GetReviewsByFarmerAsync(farmerId);

            return Ok(myReviews);
        }

        [Authorize]
        [HttpGet("getReviewById")]
        public async Task<IActionResult> GetReviewById([FromQuery] string transactionIdString)
        {
            Guid transactionId = ValidateIdString(transactionIdString);

            var review = await _reviewRepo.GetReviewByIdAsync(transactionId);

            if (review == null) return Ok(new {comment = "", rating = 0});

            return Ok(review);
        }
    }
}