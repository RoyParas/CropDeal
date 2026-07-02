using System.Security.Claims;
using Backend.DTOs.AddressDTOs;
using Backend.Exceptions;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressRepo _addressRepo;

        public AddressController(IAddressRepo addressRepo)
        {
            _addressRepo = addressRepo;
        }


        private Guid ValidateIdString(string? idString){
            if(string.IsNullOrEmpty(idString) || !Guid.TryParse(idString, out Guid Id)){
                throw new InvalidException("Invalid User Id");
            }
            return Id;
        }

        
        [Authorize(Roles = "Farmer, Dealer")]
        [HttpPost("createAddress")]
        public async Task<IActionResult> CreateAddress([FromBody] AddressDto addressDtoObj){
            var userId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var address = await _addressRepo.CreateAddressAsync(userId, addressDtoObj);
            
            var addressToShow = new ShowAddressDto().GetDto(address);
            return Ok(new {message = "Address added successfully", addressToShow});
        }


        [Authorize(Roles = "Farmer, Dealer")]
        [HttpPatch("updateAddress")]
        public async Task<IActionResult> UpdateAddress([FromBody] AddressDto updateAddressDtoObj){
            var userId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if(await _addressRepo.UpdateAddressAsync(userId, updateAddressDtoObj)){
                return Ok(new {message = "Address Updated Successfully", updateAddressDtoObj});
            }
            
            return BadRequest(new {message = "Unable to update address"});
        }


        [Authorize(Roles = "Farmer,Dealer")]
        [HttpDelete("deleteAddress")]
        public async Task<IActionResult> DeleteAddress() {
            var userId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            if(await _addressRepo.DeleteAddressAsync(userId)) return Ok(new {message ="Address deleted successfully"});
            
            return BadRequest(new {message = "Unable to delete"});
        }
    }
}