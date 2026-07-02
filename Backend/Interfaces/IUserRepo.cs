using Backend.DTOs.UserDTOs;
using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.Interfaces
{
    public interface IUserRepo
    {
        Task<List<User>> GetAllUsersAsync();

        AdminDashBoardDto GetAdminDashBoard();

        Task<FarmerDashBoardDto> GetFarmerDashBoardAsync(Guid farmerId);

        Task<DealerDashBoardDto> GetDealerDashBoardAsync(Guid dealerId);

        Task<User> GetUserByIdAsync(Guid userId);

        Task<User> GetUserAndBankAccountAndAddressByIdAsync(Guid userId);

        Task<IdentityResult> EditProfileAsync(Guid userId,  UpdateUserDto updatedUser);

        Task<IdentityResult> ChangePasswordAsync(Guid userId, PasswordDto passwordDto);
        
        Task<bool> ChangeUserStatusAsync(string email);
    }
}