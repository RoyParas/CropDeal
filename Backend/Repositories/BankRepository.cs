using System.Threading.Tasks;
using Backend.Data;
using Backend.DTOs.BankDTOs;
using Backend.Exceptions;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;
public class BankRepository : IBankRepo {

    private readonly AppDbContext _context;

    public BankRepository(AppDbContext context)
    {
        _context = context;
    }
    
    // public List<BankAccount> GetBankAccounts(){
    //     var accounts = _context.BankAccounts.ToList();

    //     if(accounts.Count <= 0) throw new NotFoundException("No Accounts Found");

    //     return accounts;
    // }

    public async Task<BankAccount> GetUserBankAccountAsync(Guid userId){
        var account = await _context.BankAccounts.FirstOrDefaultAsync(b => b.UserId == userId);

        if(account == null) throw new NotFoundException("Account not found for this user");

        return account;
    }

    public async Task<BankAccount> AddAccountAsync(Guid userId, AddBankDto bankDtoObj){
        var account = await _context.BankAccounts.FirstOrDefaultAsync(a => a.UserId == userId);

        if(account != null) throw new ExistException("Account already exist");

        account = new BankAccount{
            UserId = userId,
            BankName = bankDtoObj.BankName,
            BranchName = bankDtoObj.BranchName,
            IFSCCode = bankDtoObj.IFSCCode,
            AccountNumber = bankDtoObj.AccountNumber,
        };
        await _context.BankAccounts.AddAsync(account);

        var user = await _context.Users.FindAsync(userId);
        if(user != null) user.BankAccountId = account.Id;

        if(await _context.SaveChangesAsync() > 0) return account;

        else throw new UnableException("Unable to add bank account");
    }

    public async Task<bool> DeleteAccountAsync(Guid userId){
        var account = await _context.BankAccounts.FirstOrDefaultAsync(a => a.UserId == userId);

        if(account == null) throw new NotFoundException("No Account found");

        _context.BankAccounts.Remove(account);
        
        return await _context.SaveChangesAsync() > 0;
    }
}