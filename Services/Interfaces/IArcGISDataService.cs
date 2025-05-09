using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeterChangeApi.Models;

namespace MeterChangeApi.Services.Interfaces
{
    /// <summary>
    /// Defines the contract for a service that handles operations related to <see cref="ArcGISData"/> entities.
    /// </summary>
    public interface IArcGISDataService
    {
        /// <summary>
        /// Retrieves an <see cref="ArcGISData"/> entity by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the ArcGIS data to retrieve.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the <see cref="ArcGISData"/> if found, otherwise null.</returns>
        Task<ArcGISData> GetArcGISDataByIdAsync(int id);

        /// <summary>
        /// Retrieves all <see cref="ArcGISData"/> entities.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a collection of all <see cref="ArcGISData"/> entities.</returns>
        Task<IEnumerable<ArcGISData>> GetAllArcGISDataAsync();

        /// <summary>
        /// Retrieves a paginated list of <see cref="ArcGISData"/> entities.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of ArcGIS data items to retrieve per page.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a tuple with a list of <see cref="ArcGISData"/> entities for the specified page and the total number of ArcGIS data items.</returns>
        Task<(List<ArcGISData>, int)> GetPaginatedArcGISDataAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Creates a new <see cref="ArcGISData"/> entity.
        /// </summary>
        /// <param name="arcGISData">The <see cref="ArcGISData"/> entity to create.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the created <see cref="ArcGISData"/> entity.</returns>
        Task<ArcGISData> CreateArcGISDataAsync(ArcGISData arcGISData);

        /// <summary>
        /// Updates an existing <see cref="ArcGISData"/> entity.
        /// </summary>
        /// <param name="arcGISData">The <see cref="ArcGISData"/> entity to update.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task UpdateArcGISDataAsync(ArcGISData arcGISData);

        /// <summary>
        /// Deletes an <see cref="ArcGISData"/> entity by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the ArcGIS data to delete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task DeleteArcGISDataAsync(int id);
    }
}