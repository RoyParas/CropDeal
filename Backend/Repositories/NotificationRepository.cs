using Backend.Data;
using Backend.Exceptions;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class NotificationRepository : INotificationRepo
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetAllNotificationsAsync(Guid userId)
        {
            return await _context.Notifications.Where(n => n.UserId == userId).ToListAsync();
        }

        public async Task<bool> NotifyUserAsync(Guid userId, string message)
        {
            Notification notification = new Notification
            {
                UserId = userId,
                Message = message
            };
            await _context.Notifications.AddAsync(notification);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteNotificationAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification == null) throw new NotFoundException("Notification not found");

            _context.Notifications.Remove(notification);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}