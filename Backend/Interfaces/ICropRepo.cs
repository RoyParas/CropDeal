using Backend.DTOs.CropDTOs;
using Backend.Models;

namespace Backend.Interfaces
{
    public interface ICropRepo
    {
        List<Crop> GetCrops();

        string[] GetCropsType();

        string[] GetCropsName(string cropType);

        string[] GetDistinctCropsName();

        Task<Crop> AddCropAsync(CropDto cropDtoObj);  

        Task<bool> EditCropAsync(Guid cropId, CropDto cropDtoObj);
        
        Task<bool> DeleteCropAsync(string cropName); 
    }
}