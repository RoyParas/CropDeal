using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.DTOs.UserDTOs;
using Backend.Enums;
using Backend.Exceptions;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<User> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }


        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUpDto signUpObj) {
            
            if (!Enum.TryParse(signUpObj.Role, out UserRole role)) {
                throw new InvalidException("Invalid Role");
            }

            var checkUser = await _userManager.FindByEmailAsync(signUpObj.Email);

            if(checkUser != null) throw new ExistException("Email already exist");

            if (signUpObj.Password != signUpObj.ConfirmPassword) throw new InvalidException("Both Passwords does not match");

            var user = new User {
                UserName = signUpObj.Email,
                FullName = signUpObj.FullName,
                Email = signUpObj.Email,
                PhoneNumber = signUpObj.PhoneNumber,
                Role = signUpObj.Role
            };

            if (role == UserRole.Farmer) user.AverageRating = 0;

            var userCreated = await _userManager.CreateAsync(user, signUpObj.Password);

            if(userCreated.Succeeded){
                await _userManager.AddToRoleAsync(user, signUpObj.Role.ToString());

                var userDto = new ShowUserDto().GetDto(user);
                return Ok(new {message = "User Created Successfully", userDto});
            }
            
            else return BadRequest(new {message = userCreated.Errors.Select(e => e.Description)});
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(SignInDto signInObj){
            var user = await _userManager.FindByEmailAsync(signInObj.Email);

            if(user == null) throw new NotFoundException("User not found");

            if(!user.IsActive) {
                return StatusCode(403, new { message = "Your account has been deactivated! Contact Admin"});
            }

            if(!await _userManager.CheckPasswordAsync(user, signInObj.Password)) {
                return Unauthorized(new { message = "Invalid Credentials"});
            }

            var authClaims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = GenerateJwtToken(authClaims);
            return Ok(new {token});
        }

        // Generating Token
        private string GenerateJwtToken(List<Claim> authClaims){
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}