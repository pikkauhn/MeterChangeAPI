using MeterChangeApi.Models;

namespace MeterChangeApi.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for a repository that handles access to <see cref="Wmeter"/> entities.
    /// </summary>
    public interface IMeterRepository
    {
        /// <summary>
        /// Retrieves a <see cref="Wmeter"/> entity by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the meter to retrieve.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the <see cref="Wmeter"/> if found, otherwise null.</returns>
        Task<Wmeter> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves all <see cref="Wmeter"/> entities.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a collection of all <see cref="Wmeter"/> entities.</returns>
        Task<IEnumerable<Wmeter>> GetAllAsync();

        /// <summary>
        /// Retrieves a paginated list of <see cref="Wmeter"/> entities.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of meters to retrieve per page.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a tuple with a list of <see cref="Wmeter"/> entities for the specified page and the total number of meters.</returns>
        Task<(List<Wmeter>, int)> GetPaginatedMetersAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Adds a new <see cref="Wmeter"/> entity to the repository.
        /// </summary>
        /// <param name="meter">The <see cref="Wmeter"/> entity to add.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the added <see cref="Wmeter"/> entity.</returns>
        Task<Wmeter> AddAsync(Wmeter meter);

        /// <summary>
        /// Updates an existing <see cref="Wmeter"/> entity in the repository.
        /// </summary>
        /// <param name="meter">The <see cref="Wmeter"/> entity to update.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task UpdateAsync(Wmeter meter);

        /// <summary>
        /// Deletes a <see cref="Wmeter"/> entity from the repository by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the meter to delete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task DeleteAsync(int id);
    }
}