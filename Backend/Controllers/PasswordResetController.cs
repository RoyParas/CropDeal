using Backend.Exceptions;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordResetController : ControllerBase
    {
        private readonly IPasswordResetRepo _passwordResetRepo;
        public PasswordResetController(IPasswordResetRepo passwordResetRepo)
        {
            _passwordResetRepo = passwordResetRepo;
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> GenerateResetPasswordLinkAsync([FromForm] string email)
        {
            string token = await _passwordResetRepo.GenerateResetPasswordLinkAsync(email);
            if (token == "") throw new NotFoundException("Link Generation Failed");

            string resetLink = "http://localhost:4200/reset-password?token=" + token;
            return Ok(new { ResetLink = resetLink });
        }

        [HttpGet("verify-link")]
        public async Task<IActionResult> VerifyLinkAsync([FromQuery] string token)
        {
            if (!await _passwordResetRepo.VerifyLinkAsync(token)) return BadRequest(new { message = "The link is invalid" });

            else return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromQuery] string token, [FromBody] string newPassword)
        {
            var result = await _passwordResetRepo.ResetPasswordAsync(token, newPassword);
            if (result.Succeeded)
            {
                if (await _passwordResetRepo.InvalidateTokenAsync(token))
                    return Ok(new { message = "Password has been reset succesfully" });

                else return BadRequest(new { message = "Something went wrong! Try again later." });
            }
            else return BadRequest(new { message = result.Errors.Select(e => e.Description)});
        }

        

        [Authorize(Roles = "Admin")]
        [HttpPost("clean-expired-tokens")]
        public async Task<IActionResult> CleanExpiredTokens()
        {
            if (!await _passwordResetRepo.CleanUpExpiredTokens())
            {
                return BadRequest(new { message = "Something went wrong! Try again later." });
            }

            else return Ok(new { message = "All expired tokens has been removed successfully" });
        }
        
    }
}