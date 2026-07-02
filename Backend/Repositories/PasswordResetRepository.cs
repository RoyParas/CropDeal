using Backend.Data;
using Backend.Exceptions;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class PasswordResetRepository : IPasswordResetRepo
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public PasswordResetRepository(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<string> GenerateResetPasswordLinkAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) throw new NotFoundException("User with this email does not exist");

            PasswordResetToken resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = Guid.NewGuid()
            };
            await _context.PasswordResetTokens.AddAsync(resetToken);
            await _context.SaveChangesAsync();

            return resetToken.Token.ToString();
        }

        public async Task<bool> VerifyLinkAsync(string token)
        {
            PasswordResetToken passwordResetToken = await getPasswordResetToken(token);

            if (passwordResetToken.Expiry <= DateTime.UtcNow) throw new UnableException("The link is expired, try again to generate new link");

            if (passwordResetToken.IsUsed == true) throw new UnableException("This link has already been used once to change the password. Try generating new link");

            return true;
        }

        public async Task<IdentityResult> ResetPasswordAsync(string token, string newPassword)
        {
            PasswordResetToken passwordResetToken = await getPasswordResetToken(token);

            var user = passwordResetToken.User;

            if (user == null) throw new NotFoundException("User for the token received not found");

            var result = await _userManager.RemovePasswordAsync(user);
            if (result.Succeeded)
            {
                return await _userManager.AddPasswordAsync(user, newPassword);
            }
            return result;
        }

        public async Task<bool> InvalidateTokenAsync(string token)
        {
            PasswordResetToken passwordResetToken = await getPasswordResetToken(token);

            passwordResetToken.IsUsed = true;
            _context.PasswordResetTokens.Update(passwordResetToken);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CleanUpExpiredTokens()
        {
            var expired = await _context.PasswordResetTokens
               .Where(r => r.IsUsed || r.Expiry < DateTime.UtcNow)
               .ToListAsync();

            if (expired.Count <= 0) throw new NotFoundException("There are no Expired tokens");
            _context.PasswordResetTokens.RemoveRange(expired);
            return await _context.SaveChangesAsync() > 0;
        }

        private async Task<PasswordResetToken> getPasswordResetToken(string token) {
            var passwordResetToken = await _context.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token.ToString() == token);

            if (passwordResetToken == null) throw new NotFoundException("The link is invalid OR token not found");
            else return passwordResetToken;
        }
    }
}