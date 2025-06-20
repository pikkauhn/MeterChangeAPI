using MeterChangeApi.Models;

namespace MeterChangeApi.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for a repository that handles access to <see cref="ArcGISData"/> entities.
    /// </summary>
    public interface IArcGISDataRepository
    {
        /// <summary>
        /// Retrieves an <see cref="ArcGISData"/> entity by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the ArcGIS data to retrieve.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the <see cref="ArcGISData"/> if found, otherwise null.</returns>
        Task<ArcGISData> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves all <see cref="ArcGISData"/> entities.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a collection of all <see cref="ArcGISData"/> entities.</returns>
        Task<IEnumerable<ArcGISData>> GetAllAsync();

        /// <summary>
        /// Retrieves a paginated list of <see cref="ArcGISData"/> entities.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of ArcGIS data items to retrieve per page.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a tuple with a list of <see cref="ArcGISData"/> entities for the specified page and the total number of ArcGIS data items.</returns>
        Task<(List<ArcGISData>, int)> GetPaginatedArcGISDataAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Adds a new <see cref="ArcGISData"/> entity to the repository.
        /// </summary>
        /// <param name="arcGISData">The <see cref="ArcGISData"/> entity to add.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the added <see cref="ArcGISData"/> entity.</returns>
        Task<ArcGISData> AddAsync(ArcGISData arcGISData);

        /// <summary>
        /// Updates an existing <see cref="ArcGISData"/> entity in the repository.
        /// </summary>
        /// <param name="arcGISData">The <see cref="ArcGISData"/> entity to update.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task UpdateAsync(ArcGISData arcGISData);

        /// <summary>
        /// Deletes an <see cref="ArcGISData"/> entity from the repository by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the ArcGIS data to delete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task DeleteAsync(int id);
    }
}