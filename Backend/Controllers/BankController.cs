using System.Security.Claims;
using Backend.DTOs.BankDTOs;
using Backend.Exceptions;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankController : ControllerBase
    {
        private readonly IBankRepo _bankRepo;

        public BankController(IBankRepo bankRepo)
        {
            _bankRepo = bankRepo;
        }

        private Guid ValidateIdString(string? idString){
            if(string.IsNullOrEmpty(idString) || !Guid.TryParse(idString, out Guid Id)){
                throw new InvalidException("Invalid User Id");
            }
            return Id;
        }


        [Authorize(Roles = "Farmer,Dealer")]
        [HttpPost("addAccount")]
        public async Task<IActionResult> AddAccount([FromBody] AddBankDto bankDtoObj){
            var userId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var account = await _bankRepo.AddAccountAsync(userId, bankDtoObj);
            
            var accountToShow = new ShowBankDto().GetDto(account);
            return Ok(new {message = "Bank Account added successfully", accountToShow});
        }


        [Authorize(Roles = "Farmer,Dealer")]
        [HttpDelete("deleteAccount")]
        public async Task<IActionResult> DeleteAccount(){
            var userId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if(await _bankRepo.DeleteAccountAsync(userId)) return Ok(new {message="Account deleted successfully"});
            
            throw new UnableException("Unable to delete");
        }
    }
}