using MeterChangeApi.Models;

namespace MeterChangeApi.Services.Interfaces
{
    /// <summary>
    /// Defines the contract for a service that handles operations related to <see cref="WEndpoint"/> entities.
    /// </summary>
    public interface IEndpointService
    {
        /// <summary>
        /// Retrieves a <see cref="WEndpoint"/> entity by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the endpoint to retrieve.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the <see cref="WEndpoint"/> if found, otherwise null.</returns>
        Task<WEndpoint> GetEndpointByIdAsync(int id);

        /// <summary>
        /// Retrieves all <see cref="WEndpoint"/> entities.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a collection of all <see cref="WEndpoint"/> entities.</returns>
        Task<IEnumerable<WEndpoint>> GetAllEndpointsAsync();

        /// <summary>
        /// Retrieves a paginated list of <see cref="WEndpoint"/> entities.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of endpoints to retrieve per page.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a tuple with a list of <see cref="WEndpoint"/> entities for the specified page and the total number of endpoints.</returns>
        Task<(List<WEndpoint>, int)> GetPaginatedEndpointsAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Creates a new <see cref="WEndpoint"/> entity.
        /// </summary>
        /// <param name="endpoint">The <see cref="WEndpoint"/> entity to create.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the created <see cref="WEndpoint"/> entity.</returns>
        Task<WEndpoint> CreateEndpointAsync(WEndpoint endpoint);

        /// <summary>
        /// Updates an existing <see cref="WEndpoint"/> entity.
        /// </summary>
        /// <param name="endpoint">The <see cref="WEndpoint"/> entity to update.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task UpdateEndpointAsync(WEndpoint endpoint);

        /// <summary>
        /// Deletes a <see cref="WEndpoint"/> entity by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the endpoint to delete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task DeleteEndpointAsync(int id);
    }
}