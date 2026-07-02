
using Backend.Models;

namespace Backend.Interfaces
{
    public interface INotificationRepo
    {
        Task<List<Notification>> GetAllNotificationsAsync(Guid userId);

        Task<bool> NotifyUserAsync(Guid userId, string message);

        Task<bool> DeleteNotificationAsync(Guid notificationId);
    }
}