using Backend.Data;
using Backend.DTOs.CropDTOs;
using Backend.Exceptions;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;
public class CropRepository : ICropRepo
{
    private readonly AppDbContext _context;

    public CropRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public List<Crop> GetCrops(){
        return _context.Crops.ToList();
    }

    public string[] GetCropsType()
    {
        return _context.Crops.Select(c => c.Type).Distinct().ToArray();
    }

    public string[] GetCropsName(string cropType)
    {
        return _context.Crops.Where(c => c.Type == cropType).Select(c => c.Name).ToArray();
    }

    public string[] GetDistinctCropsName()
    {
        return _context.Crops.Select(c => c.Name).Distinct().ToArray();
    }

    public async Task<Crop> AddCropAsync(CropDto cropDtoObj)
    {
        var checkCrop = await _context.Crops.FirstOrDefaultAsync(c => c.Name == cropDtoObj.Name);

        if (checkCrop != null) throw new ExistException("Crop Already Exist");

        var crop = new Crop
        {
            Name = cropDtoObj.Name,
            Type = cropDtoObj.Type,
        };

        await _context.Crops.AddAsync(crop);
        if (await _context.SaveChangesAsync() > 0) return crop;

        else throw new UnableException("Unable to add crop");
    }

    public async Task<bool> EditCropAsync(Guid cropId, CropDto cropDtoObj){
        var crop = await _context.Crops.FirstOrDefaultAsync(c => c.Id == cropId);

        if(crop == null) throw new NotFoundException("Crop Not Found");

        crop.Name = cropDtoObj.Name;
        crop.Type = cropDtoObj.Type;
        crop.UpdatedAt = DateTime.Now;

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteCropAsync(string cropName){
        var crop = await _context.Crops.FirstOrDefaultAsync(c => c.Name == cropName);

        if(crop == null) throw new NotFoundException("Crop Not Found");

        var isCropIncludedInTransaction = await _context.Transactions
                                                        .Include(t => t.Listing)
                                                        .ThenInclude(l => l.Crop)
                                                        .FirstOrDefaultAsync(t => t.Listing != null && t.Listing.Crop != null && t.Listing.Crop.Name == cropName);

        if (isCropIncludedInTransaction != null) throw new UnableException($"Cannot Delete {cropName} as it is involved in transaction");

        _context.Crops.Remove(crop);
        
        return await _context.SaveChangesAsync() > 0;
    }
}