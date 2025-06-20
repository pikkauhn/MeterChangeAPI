using MeterChangeApi.Middleware.ExceptionHandling;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeApi.Services.Interfaces;

namespace MeterChangeApi.Services
{
    /// <summary>
    /// Implements the <see cref="IArcGISDataService"/> interface to handle operations related to ArcGIS data.
    /// This service interacts with the <see cref="IArcGISDataRepository"/> for data access and uses the
    /// <see cref="IServiceOperationHandler"/> for consistent error handling and logging.
    /// </summary>
    /// <param name="arcGISDataRepository">The repository for accessing ArcGIS data.</param>
    /// <param name="serviceOperationHandler">The handler for executing service operations with error handling.</param>
    /// <param name="logger">The logger instance for logging messages within this service.</param>
    public class ArcGISDataService(IArcGISDataRepository arcGISDataRepository, IServiceOperationHandler serviceOperationHandler, ILogger<ArcGISDataService> logger) : IArcGISDataService
    {
        private readonly IArcGISDataRepository _arcGISDataRepository = arcGISDataRepository;
        private readonly IServiceOperationHandler _serviceOperationHandler = serviceOperationHandler;
        private readonly ILogger<ArcGISDataService> _logger = logger;

        /// <inheritdoc />
        public async Task<ArcGISData> CreateArcGISDataAsync(ArcGISData arcGISData)
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _arcGISDataRepository.AddAsync(arcGISData);
            }, "Error creating ArcGIS data.");
        }

        /// <inheritdoc />
        public async Task DeleteArcGISDataAsync(int id)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _arcGISDataRepository.DeleteAsync(id);
            }, $"Error deleting ArcGIS data with ID: {id}.");
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ArcGISData>> GetAllArcGISDataAsync()
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _arcGISDataRepository.GetAllAsync();
            }, "Error retrieving all ArcGIS data.");
        }

        /// <inheritdoc />
        public async Task<ArcGISData> GetArcGISDataByIdAsync(int id)
        {
            // Input validation: Ensure the ID is a positive integer.
            if (id <= 0)
            {
                _logger.LogError("Invalid ArcGISData ID: {id}.", id);
                throw new InvalidInputException("Invalid ArcGISData ID");
            }

            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _arcGISDataRepository.GetByIdAsync(id);
            }, $"Error retrieving ArcGIS data with ID: {id}.");
        }

        /// <inheritdoc />
        public async Task<(List<ArcGISData>, int)> GetPaginatedArcGISDataAsync(int pageNumber, int pageSize)
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _arcGISDataRepository.GetPaginatedArcGISDataAsync(pageNumber, pageSize);
            }, $"Error retrieving paginated ArcGIS data (page {pageNumber}, pageSize {pageSize}).");
        }

        /// <inheritdoc />
        public async Task UpdateArcGISDataAsync(ArcGISData arcGISData)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _arcGISDataRepository.UpdateAsync(arcGISData);
            }, $"Error updating ArcGIS data with ID: {arcGISData.ArcGISDataID}.");
        }
    }
}