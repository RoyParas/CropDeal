using System.Threading.Tasks;
using Backend.Data;
using Backend.DTOs.UserDTOs;
using Backend.Exceptions;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;
public class UserRepository : IUserRepo {

    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public UserRepository(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    public async Task<List<User>> GetAllUsersAsync() {
        return await _context.Users.ToListAsync();
    }

    public AdminDashBoardDto GetAdminDashBoard()
    {
        return new AdminDashBoardDto
        {
            TotalDealers = _context.Users.Count(u => u.Role == "Dealer"),
            TotalFarmers = _context.Users.Count(u => u.Role == "Farmer"),
            TotalAllowedCrops = _context.Crops.Count(),
            TotalListedCrops = _context.CropListings.Count(cl => cl.AvailabilityStatus == true),
            CompletedDeals = _context.Transactions.Count(t => t.TransactionStatus == "Completed"),
            TotalDealValue = _context.Transactions.Where(t => t.TransactionStatus == "Completed").Sum(t => t.TotalPrice),
        };
    }

    public async Task<FarmerDashBoardDto> GetFarmerDashBoardAsync(Guid farmerId)
    {
        List<Transaction> transactions = await _context.Transactions.Include(cl => cl.Listing).Where(cl => cl.Listing.FarmerId == farmerId).ToListAsync();
        var avgRating = await _context.Users.Where(u => u.Id == farmerId)
                            .Select(u => u.AverageRating)
                            .FirstOrDefaultAsync();

        return new FarmerDashBoardDto
        {
            TotalListedCrops = _context.CropListings.Count(cl => cl.FarmerId == farmerId),
            CompletedDeals = transactions.Count(t => t.TransactionStatus == "Completed"),
            AverageRating = (float)avgRating,
            TotalEarnings = transactions.Sum(t => t.TotalPrice),
        };
    }

    public async Task<DealerDashBoardDto> GetDealerDashBoardAsync(Guid dealerId)
    {
        List<Transaction> transactions = await _context.Transactions.Where(t => t.DealerId == dealerId).ToListAsync();
        return new DealerDashBoardDto
        {
            TotalQuantityPurchased = transactions.Sum(t => t.QuantityInKg),
            CompletedDeals = transactions.Count(t => t.TransactionStatus == "Completed"),
            TotalSpent = transactions.Sum(t => t.TotalPrice),
        };
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) throw new NotFoundException("User not found");
        return user;
    }

    public async Task<User> GetUserAndBankAccountAndAddressByIdAsync(Guid userId)
    {
        var user = await _context.Users
                .Include(u => u.Address)
                .Include(u => u.BankAccount)
                .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) throw new NotFoundException("User not found");
        return user;
    }


    public async Task<IdentityResult> EditProfileAsync(Guid userId,  UpdateUserDto updatedUser){
        var user = await _context.Users.FirstAsync(u => u.Id == userId);
        user.UserName = updatedUser.Email;
        user.NormalizedUserName = _userManager.NormalizeName(updatedUser.Email);
        user.FullName = updatedUser.FullName;
        user.Email = updatedUser.Email;
        user.NormalizedEmail = _userManager.NormalizeEmail(updatedUser.Email);
        user.PhoneNumber = updatedUser.PhoneNumber; 
        user.UpdatedAt = DateTime.Now;

        return await _userManager.UpdateAsync(user);
    }


    public async Task<IdentityResult> ChangePasswordAsync(Guid userId, PasswordDto passwordDto){
        var user = await _context.Users.FirstAsync(u => u.Id == userId);

        return await _userManager.ChangePasswordAsync(user, passwordDto.OldPassword, passwordDto.NewPassword);
    }

    public async Task<bool> ChangeUserStatusAsync(string email){
            var user = await _userManager.FindByEmailAsync(email);

            if(user == null) throw new NotFoundException("User doesn't exist");

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.Now;

        return await _context.SaveChangesAsync() > 0;
    }
}