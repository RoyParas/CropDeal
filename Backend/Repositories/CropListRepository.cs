using Backend.Data;
using Backend.DTOs.CropListDTOs;
using Backend.Exceptions;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class CropListRepository : ICropListRepo
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnv;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly INotificationRepo _notificationRepo;

    public CropListRepository(AppDbContext context, IWebHostEnvironment webHostEnv, IHttpContextAccessor httpContextAccessor, INotificationRepo notificationRepo)
    {
        _context = context;
        _webHostEnv = webHostEnv;
        _httpContextAccessor = httpContextAccessor;
        _notificationRepo = notificationRepo;
    }


    public async Task<List<CropListing>> GetAllListedCropsAsync()
    {
        try
        {
            return await _context.CropListings.Include(cl => cl.Crop).OrderByDescending(cl => cl.CreatedAt).ToListAsync();
        }catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }


    public async Task<CropListing> GetListedCropByIdAsync(Guid cropListingId)
    {
        try
        {
            return await _context.CropListings
                                    .Include(cl => cl.Crop)
                                    .Include(cl => cl.Farmer)
                                    .FirstAsync(cl => cl.Id == cropListingId);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }


    public async Task<List<CropListing>>GetListedCropsByFarmerAsync(Guid farmerId) {
        try
        {
            return await _context.CropListings.Include(cl => cl.Crop).Where(cl => cl.FarmerId == farmerId).ToListAsync();
        } catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<CropListing> ListCropAsync(Guid farmerId, AddCropListDto addCropListDtoObj)
    {
        if (!await _context.Crops.AnyAsync()) throw new NotFoundException("No Crops have been added by the admin to be listed");

        if (!await _context.Addresses.AnyAsync(a => a.UserId == farmerId)) throw new UnableException("You have not added address. Please Add address to list your crops");

        if (!await _context.BankAccounts.AnyAsync(a => a.UserId == farmerId)) throw new UnableException("You have not added bank account. Please Add bank account to list your crops");


        var cropExistsInType = await _context.Crops
                                                .Where(c => c.Name == addCropListDtoObj.Name)
                                                .Select(c => c.Type)
                                                .FirstOrDefaultAsync();

        if (cropExistsInType != addCropListDtoObj.Type) throw new NotFoundException("Type Mismatch or Crop Not Found");

        if (await _context.CropListings.AnyAsync(cl => cl.Crop.Name == addCropListDtoObj.Name && cl.FarmerId == farmerId))
            throw new ExistException("Crop Already Exists! You can update Quantity");

        string imagePath = await SaveImageAsync(addCropListDtoObj.Image);

        var cropToAdd = new CropListing
        {
            FarmerId = farmerId,
            CropId = await _context.Crops
                                        .Where(c => c.Name == addCropListDtoObj.Name)
                                        .Select(c => c.Id)
                                        .FirstOrDefaultAsync(),
            PricePerKg = addCropListDtoObj.PricePerKg,
            QuantityInKg = addCropListDtoObj.QuantityInKg,
            ImageUrl = imagePath,
            Description = addCropListDtoObj.Description
        };

        await _context.CropListings.AddAsync(cropToAdd);

        if (await _context.SaveChangesAsync() > 0)
        {
            if (await _context.Subscriptions.AnyAsync(s => s.Crop.Name == addCropListDtoObj.Name))
            {
                List<Guid> userIds = await _context.Subscriptions.Where(s => s.Crop.Name == addCropListDtoObj.Name).Select(s => s.DealerId).ToListAsync();

                foreach (Guid userId in userIds)
                {
                    await _notificationRepo.NotifyUserAsync(userId, $"Your subscribed crop, {addCropListDtoObj.Name} has been added");
                }
            }
            return cropToAdd;
        }

        else throw new UnableException("Unable to add Crop to list");
    }


    public async Task<bool> UpdateListedCropAsync(Guid farmerId, Guid cropListingId, UpdateCropListDto updateCropListDtoObj)
    {
        var cropToUpdate = await _context.CropListings.FirstOrDefaultAsync(cl => cl.Id == cropListingId && cl.FarmerId == farmerId);

        if (cropToUpdate == null) throw new NotFoundException("No crop found in the list, check farmer or crop name");

        cropToUpdate.PricePerKg = updateCropListDtoObj.PricePerKg;
        cropToUpdate.QuantityInKg = updateCropListDtoObj.QuantityInKg;

        if (cropToUpdate.QuantityInKg > 0) cropToUpdate.AvailabilityStatus = true;
        else cropToUpdate.AvailabilityStatus = false;

        cropToUpdate.Description = updateCropListDtoObj.Description;
        cropToUpdate.UpdatedAt = DateTime.Now;

        return await _context.SaveChangesAsync() > 0;
    }


    public async Task<bool> UpdateListedCropImageAsync(Guid farmerId, Guid cropListingId, ImageToUpdate imageToUpdate)
    {
        var cropToUpdate = await _context.CropListings.FirstOrDefaultAsync(cl => cl.Id == cropListingId && cl.FarmerId == farmerId);

        if (cropToUpdate == null) throw new NotFoundException("No crop found in the list, check farmer or crop name");

        string? imagePath = null;
        if (imageToUpdate.Image != null)
        {
            var oldImagePath = cropToUpdate.ImageUrl;
            imagePath = await SaveImageAsync(imageToUpdate.Image);

            if (!string.IsNullOrEmpty(oldImagePath)) DeleteImage(oldImagePath);

            cropToUpdate.ImageUrl = imagePath;
        }

        cropToUpdate.UpdatedAt = DateTime.Now;

        return await _context.SaveChangesAsync() > 0;
    }


    public async Task<bool> DeleteListedCropAsync(Guid farmerId, Guid cropListingId)
    {
        var cropToDelete = await _context.CropListings.FirstOrDefaultAsync(cl => cl.Id == cropListingId && cl.FarmerId == farmerId);

        if (cropToDelete == null) throw new NotFoundException("No crop found in the list, check farmer or crop name");

        var isCropListIncludedInTransaction = await _context.Transactions.FirstOrDefaultAsync(t => t.ListingId == cropListingId);

        if (isCropListIncludedInTransaction != null) throw new UnableException("Cannot delete this crop as it is involved in transaction");

        DeleteImage(cropToDelete.ImageUrl);
        _context.CropListings.Remove(cropToDelete);

        return await _context.SaveChangesAsync() > 0;
    }


    public async Task<List<CropListing>> GetAvailableCropsAsync(Guid dealerId)
    {
        // Fetch available crops and sort by distance at the database level
        var availableCropsQuery = _context.CropListings
            .Include(cl => cl.Crop)
            .Where(c => c.AvailabilityStatus == true)
            .Include(cl => cl.Farmer)  // Ensure farmer is included for address
            .ThenInclude(f => f.Address);  // Include farmer's address for sorting

        if (!_context.Addresses.Any(a => a.UserId == dealerId))
            throw new UnableException("You have not added address. Please add the address so that you can see the listed available crops");

        var availableCrops = await availableCropsQuery.ToListAsync();

        if (availableCrops.Count == 0) return availableCrops;

        // Sort crops by distance based on address comparison
        var sortedCrops = await SortCropListings(availableCrops, dealerId);
        return sortedCrops;
    }


    public async Task<List<CropListing>> SearchCropFromListAsync(string cropName, Guid dealerId)
    {
        var crop = await _context.Crops.FirstOrDefaultAsync(c => c.Name == cropName);

        if (crop == null) throw new NotFoundException("Crop not found");

        var cropListBySearch = await _context.CropListings
                                        .Include(cl => cl.Farmer).ThenInclude(f => f.Address)
                                        .Include(cl => cl.Crop)
                                        .Where(cl => cl.Crop.Name.ToLower() == cropName.ToLower() && cl.AvailabilityStatus)
                                        .ToListAsync();

        if (cropListBySearch.Count == 0) return cropListBySearch;

        var cropListSortedByDistance = await SortCropListings(cropListBySearch, dealerId);

        return cropListSortedByDistance;
    }


    private async Task<List<CropListing>> SortCropListings(List<CropListing> cropListings, Guid dealerId)
    {
        var dealer = await _context.Users
                    .Include(d => d.Address)
                    .FirstOrDefaultAsync(u => u.Id == dealerId);

        if (dealer == null) throw new NotFoundException("Dealer not found");

        var dealerAddress = dealer.Address;
        if (dealerAddress == null) return cropListings;

        var cropListSortedByDistance = cropListings.OrderByDescending(cl => cl.CreatedAt).OrderByDescending(cl =>
        {
            var farmerAddress = cl.Farmer?.Address;

            int score = 0;

            if (farmerAddress != null && dealerAddress != null)
            {
                if (farmerAddress.State != dealerAddress.State) return score;
                score += 1;

                if (farmerAddress.District != dealerAddress.District) return score;
                score += 2;

                if (farmerAddress.City != dealerAddress.City) return score;
                score += 3;

                if (farmerAddress.ZipCode != dealerAddress.ZipCode) return score;
                score += 4;

                if (farmerAddress.Location != dealerAddress.Location) return score;
                score += 5;
            }

            else throw new NotFoundException("Dealer or Farmer Address not found");

            return score;
        }).ToList();

        return cropListSortedByDistance;
    }


    private async Task<string> SaveImageAsync(IFormFile file)
    {

        var uploadsFolder = Path.Combine(_webHostEnv.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "images");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var path = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var request = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";

        return $"{baseUrl}/images/{fileName}";
    }
    

    private void DeleteImage(string imageUrl)
    {
        var fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);
        var rootPath = _webHostEnv.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");
        var uploadsFolder = Path.Combine(rootPath, "images");
        var fullPath = Path.Combine(uploadsFolder, fileName);


        if (File.Exists(fullPath)) File.Delete(fullPath);
        else throw new UnableException("Error deleting image: file not found");
    }
}