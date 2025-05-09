using MeterChangeApi.Models;

namespace MeterChangeApi.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for a repository that handles access to <see cref="Address"/> entities,
    /// extending the generic <see cref="IRepository{Address}"/> interface.
    /// </summary>
    public interface IAddressRepository : IRepository<Address>
    {
        /// <summary>
        /// Finds addresses within a specified range (in feet) from the given geographical coordinates (X, Y).
        /// </summary>
        /// <param name="x">The X-coordinate of the center of the search range.</param>
        /// <param name="y">The Y-coordinate of the center of the search range.</param>
        /// <param name="distanceInFeet">The radius of the search range in feet.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a collection of <see cref="Address"/> entities within the specified range.</returns>
        Task<IEnumerable<Address>> GetAddressesByRangeAsync(double x, double y, double distanceInFeet);

        /// <summary>
        /// Finds all addresses located on the specified street.
        /// </summary>
        /// <param name="street">The name of the street to search for.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a collection of <see cref="Address"/> entities on the specified street.</returns>
        Task<IEnumerable<Address>> GetAddressesByStreetAsync(string street);

        /// <summary>
        /// Retrieves a paginated list of addresses.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of addresses to retrieve per page.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a tuple with a list of <see cref="Address"/> entities for the specified page and the total number of addresses.</returns>
        Task<(List<Address>, int)> GetPaginatedAddressesAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Finds an address by its unique Location Identification Code (ICN).
        /// </summary>
        /// <param name="locationIcn">The Location ICN to search for.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the <see cref="Address"/> entity with the matching ICN, or null if not found.</returns>
        Task<Address> GetAddressByLocationIcnAsync(int locationIcn);

        /// <summary>
        /// Finds all addresses with a specific building status.
        /// </summary>
        /// <param name="buildingStatus">The building status to filter by.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a collection of <see cref="Address"/> entities with the specified building status.</returns>
        Task<IEnumerable<Address>> GetAddressesByBuildingStatusAsync(string buildingStatus);
    }
}