using MeterChangeApi.Models;

namespace MeterChangeApi.Services.Interfaces
{
    public interface IAddressService
    {
        Task<Address> GetAddressByIdAsync(int id);
        Task<IEnumerable<Address>> GetAllAddressesAsync();
        Task<(List<Address>, int)> GetPaginatedAddressesAsync(int pageNumber, int pageSize);
        Task<MemoryStream> ExportAddressesToJsonAsync();
        Task AddAddressAsync(Address address);
        Task UpdateAddressAsync(Address address);
        Task DeleteAddressAsync(int id);
        Task<IEnumerable<Address>> GetAddressesByRangeAsync(double x, double y, double distanceInFeet);
        Task<IEnumerable<Address>> GetAddressesByStreetAsync(string street);
        Task<Address> GetAddressByLocationIcnAsync(int locationIcn);
        Task<IEnumerable<Address>> GetAddressesByBuildingStatusAsync(string buildingStatus);
    }
}