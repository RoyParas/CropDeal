using Backend.DTOs.UserDTOs;
using Backend.Models;

namespace Backend.DTOs.AddressDTOs;

public class ShowAddressDto{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public ShowUserDto? User { get; set; } 

    public string Location { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string District { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;

    public ShowAddressDto GetDto(Address address){
        Id = address.Id;
        Location = address.Location;
        City = address.City;
        District = address.District;
        State = address.State;
        ZipCode = address.ZipCode;
        UserId = address.UserId;
        User = new ShowUserDto().GetDto(address.User);

        return this;
    }
}