using Backend.Models;

namespace Backend.Interfaces
{
    public interface ISubscriptionRepo
    {
        Task<List<Subscription>> GetSubscriptionsByDealerAsync(Guid dealerId);

        Task<Subscription> AddSubscriptionAsync(Guid dealerId, string CropName);

        Task<bool> DeleteSubscriptionAsync(Guid subscriptionId);
    }
}