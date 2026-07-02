using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.Interfaces
{
    public interface IPasswordResetRepo
    {
        Task<string> GenerateResetPasswordLinkAsync(string email);
        Task<bool> VerifyLinkAsync(string token);
        Task<IdentityResult> ResetPasswordAsync(string token, string newPassword);
        Task<bool> InvalidateTokenAsync(string token);
        Task<bool> CleanUpExpiredTokens();
    }
}