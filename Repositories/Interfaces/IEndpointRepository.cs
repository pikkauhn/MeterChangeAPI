using MeterChangeApi.Models;

namespace MeterChangeApi.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for a repository that handles access to <see cref="WEndpoint"/> entities.
    /// </summary>
    public interface IEndpointRepository
    {
        /// <summary>
        /// Retrieves a <see cref="WEndpoint"/> entity by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the endpoint to retrieve.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the <see cref="WEndpoint"/> if found, otherwise null.</returns>
        Task<WEndpoint> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves all <see cref="WEndpoint"/> entities.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a collection of all <see cref="WEndpoint"/> entities.</returns>
        Task<IEnumerable<WEndpoint>> GetAllAsync();

        /// <summary>
        /// Retrieves a paginated list of <see cref="WEndpoint"/> entities.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of endpoints to retrieve per page.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a tuple with a list of <see cref="WEndpoint"/> entities for the specified page and the total number of endpoints.</returns>
        Task<(List<WEndpoint>, int)> GetPaginatedEndpointsAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Adds a new <see cref="WEndpoint"/> entity to the repository.
        /// </summary>
        /// <param name="endpoint">The <see cref="WEndpoint"/> entity to add.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the added <see cref="WEndpoint"/> entity.</returns>
        Task<WEndpoint> AddAsync(WEndpoint endpoint);

        /// <summary>
        /// Updates an existing <see cref="WEndpoint"/> entity in the repository.
        /// </summary>
        /// <param name="endpoint">The <see cref="WEndpoint"/> entity to update.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task UpdateAsync(WEndpoint endpoint);

        /// <summary>
        /// Deletes a <see cref="WEndpoint"/> entity from the repository by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the endpoint to delete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task DeleteAsync(int id);
    }
}