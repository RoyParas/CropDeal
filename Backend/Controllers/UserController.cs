using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.DTOs.UserDTOs;
using Backend.Interfaces;
using Backend.Exceptions;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;

        public UserController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        
        private Guid ValidateIdString(string? idString){
            if(string.IsNullOrEmpty(idString) || !Guid.TryParse(idString, out Guid Id)){
                throw new InvalidException("Invalid User Id");
            }
            return Id;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("getAllUser")]
        public async Task<IActionResult> GetAllUsers(){
            var usersList = await _userRepo.GetAllUsersAsync();
            return Ok(usersList);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("getAdminDashBoard")]
        public IActionResult GetAdminDashBoard(){
            AdminDashBoardDto dto = _userRepo.GetAdminDashBoard();
            return Ok(dto);
        }


        [Authorize(Roles = "Farmer")]
        [HttpGet("getFarmerDashBoard")]
        public async Task<IActionResult> GetFarmerDashBoard() {
            var userId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));
            FarmerDashBoardDto dto = await _userRepo.GetFarmerDashBoardAsync(userId);
            return Ok(dto);
        }


        [Authorize(Roles = "Dealer")]
        [HttpGet("getDealerDashBoard")]
        public async Task<IActionResult> GetDealerDashBoard() {
            var userId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));
            DealerDashBoardDto dto = await _userRepo.GetDealerDashBoardAsync(userId);
            return Ok(dto);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("getUserById")]
        public async Task<IActionResult> GetUserById([FromQuery] string userIdString) {
            var userId = ValidateIdString(userIdString);
            var user = await _userRepo.GetUserAndBankAccountAndAddressByIdAsync(userId);
            return Ok(user);
        }

        [Authorize]
        [HttpGet("myProfile")]
        public async Task<IActionResult> GetLoggedInUser() {
            var userId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userRepo.GetUserAndBankAccountAndAddressByIdAsync(userId);
            return Ok(user);
        }

        [Authorize]
        [HttpPatch("editProfile")]
        public async Task<IActionResult> EditProfile([FromBody] UpdateUserDto updatedUser){
            var userId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var isProfileEdited = await _userRepo.EditProfileAsync(userId, updatedUser);

            if(isProfileEdited.Succeeded){
                var userDto = new UpdateUserDto().GetUpdatedUserDto(updatedUser);
                return Ok(new {message = "Profile Updated Successfully", userDto});
            }
            else return BadRequest(new {message = isProfileEdited.Errors.Select(e => e.Description)});
        }

        
        [Authorize]
        [HttpPatch("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordDto passwordDto){
            var userId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var isPasswordCahnged = await _userRepo.ChangePasswordAsync(userId, passwordDto);
            
            if(isPasswordCahnged.Succeeded) return Ok(new {message = "Password Changed Successfully"});
            
            else return BadRequest(new {message = isPasswordCahnged.Errors.Select(e => e.Description)});
        }


        
        [Authorize(Roles = "Admin")]
        [HttpPatch("changeUserStatus")]
        public async Task<IActionResult> ChangeUserStatus([FromQuery]string email){
            bool isStatusChanged = await _userRepo.ChangeUserStatusAsync(email);
            if(isStatusChanged){
                return Ok(new {message = "Status Changed Successfully"});
            }
            else throw new UnableException("Unable to change the status of user"); 
        }
    }
}