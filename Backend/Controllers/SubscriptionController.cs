using System.Security.Claims;
using Backend.Exceptions;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionRepo _subRepo;

        public SubscriptionController(ISubscriptionRepo subRepo)
        {
            _subRepo = subRepo;
        }

        private Guid ValidateIdString(string? idString)
        {
            if (string.IsNullOrEmpty(idString) || !Guid.TryParse(idString, out Guid id))
            {
                throw new InvalidException($"Invalid `{idString}` id string");
            }
            return id;
        }

        [Authorize(Roles = "Dealer")]
        [HttpGet("getSubscribedCropsByDealer")]
        public async Task<IActionResult> getSubscribedCropsByDealer()
        {
            var dealerId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            List<Subscription> subscribedCrops = await _subRepo.GetSubscriptionsByDealerAsync(dealerId);

            return Ok(subscribedCrops);
        }

        [Authorize(Roles = "Dealer")]
        [HttpPost("addSubscription")]
        public async Task<IActionResult> addSubscription([FromQuery] string CropName)
        {
            var dealerId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var SubscriptionAdded = await _subRepo.AddSubscriptionAsync(dealerId, CropName);

            return Ok(new { message = "Subscribed successfully", SubscriptionAdded });
        }

        [Authorize(Roles = "Dealer")]
        [HttpDelete("deleteSubscription")]
        public async Task<IActionResult> deleteSubscription([FromQuery] string subscriptionIdString)
        {
            var subscriptionId = ValidateIdString(subscriptionIdString);

            if (await _subRepo.DeleteSubscriptionAsync(subscriptionId))
                return Ok(new { message = "Subscription Deleted Successfully" });

            else throw new UnableException("Unable to delete subscription");
        }

    }
}