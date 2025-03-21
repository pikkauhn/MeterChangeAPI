using MeterChangeApi.Models;

namespace MeterChangeApi.Repositories.Interfaces
{
    public interface IAddressRepository : IRepository<Address>
    {                
        // Find addresses in a range based on given geocoordinates.
        Task<IEnumerable<Address>> GetAddressesByRangeAsync(double x, double y, double distanceInFeet);
        // Find addresses by street name
        Task<IEnumerable<Address>> GetAddressesByStreetAsync(string street);
        // Find addresses by pagination
        Task<(List<Address>, int)> GetPaginatedAddressesAsync(int pageNumber, int pageSize);
        // Find addresses by location ICN
        Task<Address> GetAddressByLocationIcnAsync(int locationIcn);

        //Find Addresses with a specific Building Status
        Task<IEnumerable<Address>> GetAddressesByBuildingStatusAsync(string buildingStatus);
            
    }
}