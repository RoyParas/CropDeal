using Backend.DTOs.CropDTOs;
using Backend.Enums;
using Backend.Exceptions;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CropController : ControllerBase
    {
        private readonly ICropRepo _cropRepo;

        public CropController(ICropRepo cropRepo)
        {
            _cropRepo = cropRepo;
        }

        private Guid ValidateIdString(string? IdString){
            if(string.IsNullOrEmpty(IdString) || !Guid.TryParse(IdString, out Guid Id)){
                throw new InvalidException($"Invalid `{IdString}` Id string");
            }
            return Id;
        }

        [HttpGet("getAllCrops")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllCrops(){
            var allCrops = _cropRepo.GetCrops();
            return Ok(allCrops);
        }

        [HttpGet("getCropsType")]
        [Authorize(Roles = "Farmer")]
        public IActionResult GetCropsType()
        {
            string[] cropsType = _cropRepo.GetCropsType();
            if (!cropsType.Any()) return NotFound("No Crop Types found");
            return Ok(cropsType);
        }

        [HttpGet("getCropsName")]
        [Authorize(Roles = "Farmer")]
        public IActionResult GetCropsName([FromQuery] string cropType)
        {
            string[] cropsName = _cropRepo.GetCropsName(cropType);
            if (!cropsName.Any()) return NotFound("No Crops found");
            return Ok(cropsName);
        }

        [HttpGet("getDistinctCropsName")]
        [Authorize(Roles = "Dealer")]
        public IActionResult GetDistinctCropsName()
        {
            string[] cropsNames = _cropRepo.GetDistinctCropsName();
            return Ok(cropsNames);
        }


        [HttpPost("addCrop")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCrop([FromBody] CropDto cropDtoObj)
        {
            if (!Enum.TryParse(cropDtoObj.Type, out CropType _)) throw new InvalidException("Invalid CropType");

            var crop = await _cropRepo.AddCropAsync(cropDtoObj);

            return Ok(new { message = "Crop Added Successfully", crop });
        }

        [HttpPatch("editCrop")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditCrop([FromQuery] string cropIdString, [FromBody] CropDto cropDtoObj){
            if(!Enum.IsDefined(typeof(CropType), cropDtoObj.Type)) throw new InvalidException("Invalid CropType");
            
            var cropId = ValidateIdString(cropIdString);

            if(await _cropRepo.EditCropAsync(cropId, cropDtoObj)){
                return Ok(new {message = "Crop Edited Successfully", cropDtoObj});
            }
            else throw new UnableException("Unable to edit crop details");
        }

        [HttpDelete("deleteCrop")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCrop([FromQuery] string cropName)
        {
            if (await _cropRepo.DeleteCropAsync(cropName))
            {
                return Ok(new { message = "Crop Deleted Successfully" });
            }

            else throw new UnableException("Unable to delete crop");
        }
    }
}