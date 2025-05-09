using MeterChangeApi.Models;

namespace MeterChangeApi.Services.Interfaces
{
    /// <summary>
    /// Defines the contract for a service that handles operations related to <see cref="Wmeter"/> entities.
    /// </summary>
    public interface IMeterService
    {
        /// <summary>
        /// Retrieves a <see cref="Wmeter"/> entity by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the meter to retrieve.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the <see cref="Wmeter"/> if found, otherwise null.</returns>
        Task<Wmeter> GetMeterByIdAsync(int id);

        /// <summary>
        /// Retrieves all <see cref="Wmeter"/> entities.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a collection of all <see cref="Wmeter"/> entities.</returns>
        Task<IEnumerable<Wmeter>> GetAllMetersAsync();

        /// <summary>
        /// Retrieves a paginated list of <see cref="Wmeter"/> entities.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of meters to retrieve per page.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a tuple with a list of <see cref="Wmeter"/> entities for the specified page and the total number of meters.</returns>
        Task<(List<Wmeter>, int)> GetPaginatedMetersAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Creates a new <see cref="Wmeter"/> entity.
        /// </summary>
        /// <param name="meter">The <see cref="Wmeter"/> entity to create.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the created <see cref="Wmeter"/> entity.</returns>
        Task<Wmeter> CreateMeterAsync(Wmeter meter);

        /// <summary>
        /// Updates an existing <see cref="Wmeter"/> entity.
        /// </summary>
        /// <param name="meter">The <see cref="Wmeter"/> entity to update.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task UpdateMeterAsync(Wmeter meter);

        /// <summary>
        /// Deletes a <see cref="Wmeter"/> entity by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the meter to delete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task DeleteMeterAsync(int id);
    }
}
