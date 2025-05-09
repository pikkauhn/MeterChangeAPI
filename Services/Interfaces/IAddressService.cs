using MeterChangeApi.Models;

namespace MeterChangeApi.Services.Interfaces
{
    /// <summary>
    /// Defines the contract for a service that handles operations related to <see cref="Address"/> entities.
    /// </summary>
    public interface IAddressService
    {
        /// <summary>
        /// Retrieves an <see cref="Address"/> by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the address to retrieve.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the <see cref="Address"/> if found, otherwise null.</returns>
        Task<Address> GetAddressByIdAsync(int id);

        /// <summary>
        /// Retrieves all <see cref="Address"/> entities.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a collection of all <see cref="Address"/> entities.</returns>
        Task<IEnumerable<Address>> GetAllAddressesAsync();

        /// <summary>
        /// Retrieves a paginated list of <see cref="Address"/> entities.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of addresses to retrieve per page.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a tuple with a list of <see cref="Address"/> entities for the specified page and the total number of addresses.</returns>
        Task<(List<Address>, int)> GetPaginatedAddressesAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Exports all <see cref="Address"/> entities to a JSON format in a <see cref="MemoryStream"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a <see cref="MemoryStream"/> containing the JSON representation of all addresses.</returns>
        Task<MemoryStream> ExportAddressesToJsonAsync();

        /// <summary>
        /// Adds a new <see cref="Address"/> entity.
        /// </summary>
        /// <param name="address">The <see cref="Address"/> entity to add.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task AddAddressAsync(Address address);

        /// <summary>
        /// Updates an existing <see cref="Address"/> entity.
        /// </summary>
        /// <param name="address">The <see cref="Address"/> entity to update.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task UpdateAddressAsync(Address address);

        /// <summary>
        /// Deletes an <see cref="Address"/> entity by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the address to delete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task DeleteAddressAsync(int id);

        /// <summary>
        /// Retrieves <see cref="Address"/> entities within a specified distance from a given coordinate.
        /// </summary>
        /// <param name="x">The X-coordinate.</param>
        /// <param name="y">The Y-coordinate.</param>
        /// <param name="distanceInFeet">The distance in feet from the coordinate to search within.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a collection of <see cref="Address"/> entities within the specified range.</returns>
        Task<IEnumerable<Address>> GetAddressesByRangeAsync(double x, double y, double distanceInFeet);

        /// <summary>
        /// Retrieves <see cref="Address"/> entities located on a specific street.
        /// </summary>
        /// <param name="street">The name of the street to search for.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a collection of <see cref="Address"/> entities on the specified street.</returns>
        Task<IEnumerable<Address>> GetAddressesByStreetAsync(string street);

        /// <summary>
        /// Retrieves an <see cref="Address"/> by its Location Identification Code (ICN).
        /// </summary>
        /// <param name="locationIcn">The Location ICN of the address to retrieve.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the <see cref="Address"/> if found, otherwise null.</returns>
        Task<Address> GetAddressByLocationIcnAsync(int locationIcn);

        /// <summary>
        /// Retrieves <see cref="Address"/> entities with a specific building status.
        /// </summary>
        /// <param name="buildingStatus">The building status to filter by.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a collection of <see cref="Address"/> entities with the specified building status.</returns>
        Task<IEnumerable<Address>> GetAddressesByBuildingStatusAsync(string buildingStatus);
    }
}