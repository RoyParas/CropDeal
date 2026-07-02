using System.Security.Claims;
using Backend.DTOs.CropListDTOs;
using Backend.Exceptions;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CropListController : ControllerBase
    {
        private readonly ICropListRepo _cropListRepo;
        private readonly IAddressRepo _addressRepo;

        public CropListController(ICropListRepo cropListRepo, IAddressRepo addressRepo)
        {
            _cropListRepo = cropListRepo;
            _addressRepo = addressRepo;
        }

        private Guid ValidateIdString(string? IdString)
        {
            if (string.IsNullOrEmpty(IdString) || !Guid.TryParse(IdString, out Guid Id))
            {
                throw new InvalidException($"Invalid `{IdString}` Id string");
            }
            return Id;
        }


        //Admin gets to see all listed crops
        [Authorize(Roles = "Admin")]
        [HttpGet("getAllListedCrops")]
        public async Task<IActionResult> GetAllListedCrops()
        {
            var allListedCrops = await _cropListRepo.GetAllListedCropsAsync();
            return Ok(allListedCrops);
        }


        [Authorize(Roles = "Admin, Dealer")]
        [HttpGet("getListedCropById")]
        public async Task<IActionResult> GetListedCropById([FromQuery] string cropListingIdString)
        {
            var cropListingId = ValidateIdString(cropListingIdString);

            var listedCropFound = await _cropListRepo.GetListedCropByIdAsync(cropListingId);

            return Ok(listedCropFound);
        }


        //Farmer sees Posted Crops 
        [Authorize(Roles = "Farmer")]
        [HttpGet("getListedCropsByFarmer")]
        public async Task<IActionResult> GetListedCropsByFarmer()
        {
            var userId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var cropsPosted = await _cropListRepo.GetListedCropsByFarmerAsync(userId);

            return Ok(cropsPosted);
        }


        //Farmer Adds Crops 
        [Authorize(Roles = "Farmer")]
        [HttpPost("listCrop")]
        public async Task<IActionResult> ListCrop([FromForm] AddCropListDto addCropListDtoObj)
        {
            var userId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var addedCropToList = await _cropListRepo.ListCropAsync(userId, addCropListDtoObj);

            return Ok(new { message = "Crop Added Successfully to list", addedCropToList });
        }


        //Farmer Updates Crops 
        [Authorize(Roles = "Farmer")]
        [HttpPatch("updateListedCrop")]
        public async Task<IActionResult> UpdateListedCrop([FromForm] UpdateCropListDto updateCropListDtoObj, [FromQuery] string cropListingIdString)
        {
            var userId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var cropListingId = ValidateIdString(cropListingIdString);

            if (await _cropListRepo.UpdateListedCropAsync(userId, cropListingId, updateCropListDtoObj))
            {
                return Ok(new { message = "Crop Updated Successfully in the list", updateCropListDtoObj });
            }

            else throw new UnableException("Unable to update details of the crop in the list");
        }


        [Authorize(Roles = "Farmer")]
        [HttpPatch("updateImageOfListedCrop")]
        public async Task<IActionResult> UpdateListedCropImage([FromForm] ImageToUpdate image, [FromQuery] string cropListingIdString)
        {
            var userId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var cropListingId = ValidateIdString(cropListingIdString);

            if (await _cropListRepo.UpdateListedCropImageAsync(userId, cropListingId, image))
            {
                return Ok(new { message = "Crop Image Updated Successfully" });
            } 
            else throw new UnableException("Unable to update image of crop");       
        }


        //Farmer Deletes Crops 
        [Authorize(Roles = "Farmer")]
        [HttpDelete("deleteListedCrop")]
        public async Task<IActionResult> DeleteListedCrop([FromQuery] string cropListingIdString)
        {
            var userId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var cropListingId = ValidateIdString(cropListingIdString);

            if (await _cropListRepo.DeleteListedCropAsync(userId, cropListingId))
            {
                return Ok(new { message = "Listed Crop deleted successfully" });
            }

            else throw new UnableException("Unable to delete crop from the list");
        }


        //Dealer gets to see all listed crops sorted on location of dealer
        [Authorize(Roles = "Dealer")]
        [HttpGet("getAvailableCropsFromTheList")]
        public async Task<IActionResult> GetAvailableCrops()
        {
            var dealerId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var availableCrops = await _cropListRepo.GetAvailableCropsAsync(dealerId);
            var dealerAddress = await _addressRepo.GetUserAddressAsync(dealerId);

            List<ShowCropListDto> availableCropsDtoList = new List<ShowCropListDto>();
            foreach (var crop in availableCrops)
            {
                availableCropsDtoList.Add(new ShowCropListDto().GetDto(crop, dealerAddress));
            }

            return Ok(availableCropsDtoList);
        }


        // Dealer search for a crop and if it is not available in the listing Dealer subscribes to the particular crop
        [Authorize(Roles = "Dealer")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchCropFromList([FromQuery] string cropName)
        {
            var dealerId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (string.IsNullOrWhiteSpace(cropName)) return BadRequest("Cropname is required");

            var cropsFound = await _cropListRepo.SearchCropFromListAsync(cropName, dealerId);
            var dealerAddress = await _addressRepo.GetUserAddressAsync(dealerId);

            List<ShowCropListDto> cropsFoundDtoList = new List<ShowCropListDto>();
            foreach (var crop in cropsFound)
            {
                cropsFoundDtoList.Add(new ShowCropListDto().GetDto(crop, dealerAddress));
            }

            return Ok(cropsFoundDtoList);
        }

        //Dealer Buys Crop
        //Farmer Updates Crops Price
        
    }
}