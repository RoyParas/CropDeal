using Backend.DTOs.AddressDTOs;
using Backend.Models;

namespace Backend.Interfaces
{
    public interface IAddressRepo
    {
        // List<Address> GetAllAddresses();

        Task<Address> GetUserAddressAsync(Guid userId);

        Task<Address> CreateAddressAsync(Guid userId, AddressDto addressDtoObj);

        Task<bool> UpdateAddressAsync(Guid userId,  AddressDto addressDtoObj);
        
        Task<bool> DeleteAddressAsync(Guid userId);
    }
}