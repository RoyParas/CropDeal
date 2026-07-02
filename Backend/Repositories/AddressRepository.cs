using Backend.Data;
using Backend.DTOs.AddressDTOs;
using Backend.Exceptions;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class AddressRepository : IAddressRepo
{
    private readonly AppDbContext _context;

    public AddressRepository(AppDbContext context)
    {
        _context = context;
    }


    // public List<Address> GetAllAddresses(){
    //     var addresses = _context.Addresses.ToList();

    //     if(addresses.Count <= 0) throw new NotFoundException("No addresses found");

    //     return addresses;
    // }


    public async Task<Address> GetUserAddressAsync(Guid userId){
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);

        if(address == null) throw new NotFoundException("Address not found for this user");

        return address;
    }

    
    public async Task<Address> CreateAddressAsync(Guid userId, AddressDto addressDtoObj){
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);

        if(address != null) throw new ExistException("Address already exist");

        address = new Address{
            UserId = userId,
            Location = addressDtoObj.Location,
            City = addressDtoObj.City,
            District = addressDtoObj.District,
            State = addressDtoObj.State,
            ZipCode = addressDtoObj.ZipCode,
        };
        await _context.Addresses.AddAsync(address);

        var user = await _context.Users.FindAsync(userId);
        if(user!= null) user.AddressId = address.Id;

        if(await _context.SaveChangesAsync() > 0) return address;

        else throw new UnableException("Unable to add address");
    }


    public async Task<bool> UpdateAddressAsync(Guid userId,  AddressDto addressDtoObj){
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);

        if(address == null) throw new NotFoundException("Address Not Found");
        
        address.Location = addressDtoObj.Location;
        address.City = addressDtoObj.City;
        address.District = addressDtoObj.District;
        address.State = addressDtoObj.State;
        address.ZipCode = addressDtoObj.ZipCode;
        // address.UpdatedAt = DateTime.Now;

        return await _context.SaveChangesAsync() > 0;
    }


    public async Task<bool> DeleteAddressAsync(Guid userId){
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);

        if(address == null) throw new NotFoundException("No Address found");

        _context.Addresses.Remove(address);
        
        return await _context.SaveChangesAsync() > 0;
    }
}