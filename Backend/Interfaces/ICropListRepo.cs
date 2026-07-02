using Backend.DTOs.CropListDTOs;
using Backend.Models;

namespace Backend.Interfaces;

public interface ICropListRepo{
    Task<List<CropListing>> GetAllListedCropsAsync();

    Task<CropListing> GetListedCropByIdAsync(Guid cropListingId);

    Task<List<CropListing>> GetListedCropsByFarmerAsync(Guid farmerId);
    
    Task<CropListing> ListCropAsync(Guid farmerId, AddCropListDto addCropListDtoObj);
    
    Task<bool> UpdateListedCropAsync(Guid farmerId, Guid cropListingId, UpdateCropListDto updateCropListDtoObj);

    Task<bool> UpdateListedCropImageAsync(Guid farmerId, Guid cropListingId, ImageToUpdate imageToUpdate);
    
    Task<bool> DeleteListedCropAsync(Guid farmerId, Guid cropListingId);
    
    Task<List<CropListing>> GetAvailableCropsAsync(Guid dealerId);
    
    Task<List<CropListing>> SearchCropFromListAsync(string cropName, Guid dealerId);
}