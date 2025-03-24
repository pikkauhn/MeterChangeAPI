using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Middleware.ExceptionHandling;

namespace MeterChangeApi.Services
{
    public class ArcGISDataService : IArcGISDataService
    {
        private readonly IArcGISDataRepository _arcGISDataRepository;
        private readonly IServiceOperationHandler _serviceOperationHandler;
        private readonly ILogger<ArcGISDataService> _logger;

        public ArcGISDataService(IArcGISDataRepository arcGISDataRepository, IServiceOperationHandler serviceOperationHandler, ILogger<ArcGISDataService> logger)
        {
            _arcGISDataRepository = arcGISDataRepository;
            _serviceOperationHandler = serviceOperationHandler;
            _logger = logger;
        }

        public async Task<ArcGISData> CreateArcGISDataAsync(ArcGISData arcGISData)
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _arcGISDataRepository.AddAsync(arcGISData);
            }, "Error creating ArcGIS data.");
        }

        public async Task DeleteArcGISDataAsync(int id)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _arcGISDataRepository.DeleteAsync(id);
            }, $"Error deleting ArcGIS data with ID: {id}.");
        }

        public async Task<IEnumerable<ArcGISData>> GetAllArcGISDataAsync()
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _arcGISDataRepository.GetAllAsync();
            }, "Error retrieving all ArcGIS data.");
        }

        public async Task<ArcGISData> GetArcGISDataByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"Invalid ArcGISData ID: {id}.");
                throw new InvalidInputException("Invalid ArcGISData ID");
            }

            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _arcGISDataRepository.GetByIdAsync(id);
            }, $"Error retrieving ArcGIS data with ID: {id}.");
        }

        public async Task<(List<ArcGISData>, int)> GetPaginatedArcGISDataAsync(int pageNumber, int pageSize)
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _arcGISDataRepository.GetPaginatedArcGISDataAsync(pageNumber, pageSize);
            }, $"Error retrieving paginated ArcGIS data (page {pageNumber}, pageSize {pageSize}).");
        }

        public async Task UpdateArcGISDataAsync(ArcGISData arcGISData)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _arcGISDataRepository.UpdateAsync(arcGISData);
            }, $"Error updating ArcGIS data with ID: {arcGISData.ArcGISDataID}.");
        }
    }
}