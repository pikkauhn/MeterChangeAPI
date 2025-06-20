using MeterChangeApi.Models;

namespace MeterChangeApi.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for a repository that handles access to <see cref="WMeter"/> entities.
    /// </summary>
    public interface IMeterRepository
    {
        /// <summary>
        /// Retrieves a <see cref="WMeter"/> entity by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the meter to retrieve.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the <see cref="WMeter"/> if found, otherwise null.</returns>
        Task<WMeter> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves all <see cref="WMeter"/> entities.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a collection of all <see cref="WMeter"/> entities.</returns>
        Task<IEnumerable<WMeter>> GetAllAsync();

        /// <summary>
        /// Retrieves a paginated list of <see cref="WMeter"/> entities.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of meters to retrieve per page.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a tuple with a list of <see cref="WMeter"/> entities for the specified page and the total number of meters.</returns>
        Task<(List<WMeter>, int)> GetPaginatedMetersAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Adds a new <see cref="WMeter"/> entity to the repository.
        /// </summary>
        /// <param name="meter">The <see cref="WMeter"/> entity to add.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the added <see cref="WMeter"/> entity.</returns>
        Task<WMeter> AddAsync(WMeter meter);

        /// <summary>
        /// Updates an existing <see cref="WMeter"/> entity in the repository.
        /// </summary>
        /// <param name="meter">The <see cref="WMeter"/> entity to update.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task UpdateAsync(WMeter meter);

        /// <summary>
        /// Deletes a <see cref="WMeter"/> entity from the repository by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the meter to delete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task DeleteAsync(int id);
    }
}