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
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepo _notificationRepo;
        public NotificationController(INotificationRepo notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        private Guid ValidateIdString(string? IdString)
        {
            if (string.IsNullOrEmpty(IdString) || !Guid.TryParse(IdString, out Guid Id))
            {
                throw new InvalidException($"Invalid `{IdString}` Id string");
            }
            return Id;
        }

        [Authorize(Roles = "Dealer")]
        [HttpGet("getNotifications")]
        public async Task<IActionResult> GetAllNotifications()
        {
            Guid userId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            List<Notification> notifications = await _notificationRepo.GetAllNotificationsAsync(userId);
            
            return Ok(notifications);
        }

        [Authorize(Roles = "Dealer")]
        [HttpDelete("deleteNotification")]
        public async Task<IActionResult> DeleteNotification([FromQuery] string notificationIdString)
        {
            Guid notificationId = ValidateIdString(notificationIdString);

            if (await _notificationRepo.DeleteNotificationAsync(notificationId))
            {
                return Ok(new { message = "Notification deleted successfully" });
            }

            throw new UnableException("Unable to delete notification");
        }
        
    }
}