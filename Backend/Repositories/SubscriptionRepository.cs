using Backend.Data;
using Backend.Exceptions;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class SubscriptionRepository : ISubscriptionRepo
{

    private readonly AppDbContext _context;

    public SubscriptionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Subscription>> GetSubscriptionsByDealerAsync(Guid dealerId)
    {
        return await _context.Subscriptions
                        .Include(s => s.Crop)
                        .Where(s => s.DealerId == dealerId)
                        .OrderByDescending(s => s.CreatedAt)
                        .ToListAsync();
    }


    public async Task<Subscription> AddSubscriptionAsync(Guid dealerId, string CropName)
    {
        var cropId = await _context.Crops
                            .Where(c => c.Name == CropName)
                            .Select(c => c.Id)
                            .FirstOrDefaultAsync();

        if (cropId == Guid.Empty)
            throw new NotFoundException("Crop Not Found to subscribe");

        if (await _context.Subscriptions.AnyAsync(s => s.CropId == cropId && s.DealerId == dealerId))
            throw new ExistException("Already Subscribed, please wait for crop to be listed");

        Subscription sub = new Subscription
        {
            DealerId = dealerId,
            CropId = cropId
        };

        await _context.Subscriptions.AddAsync(sub);
        if (await _context.SaveChangesAsync() > 0) return sub;

        else throw new UnableException("Unable to subscribe");
    }

    public async Task<bool> DeleteSubscriptionAsync(Guid subscriptionId)
    {
        var subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == subscriptionId);

        if(subscription == null) throw new NotFoundException("Crop Not Found");

        _context.Subscriptions.Remove(subscription);
        
        return await _context.SaveChangesAsync() > 0;
    }
}