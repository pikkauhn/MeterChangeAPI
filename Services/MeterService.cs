using MeterChangeApi.Middleware.ExceptionHandling;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeApi.Services.Interfaces;

namespace MeterChangeApi.Services
{
    /// <summary>
    /// Implements the <see cref="IMeterService"/> interface to handle operations related to water meters.
    /// This service interacts with the <see cref="IMeterRepository"/> for data access and uses the
    /// <see cref="IServiceOperationHandler"/> for consistent error handling and logging.
    /// </summary>
    /// <param name="meterRepository">The repository for accessing water meter data.</param>
    /// <param name="serviceOperationHandler">The handler for executing service operations with error handling.</param>
    /// <param name="logger">The logger instance for logging messages within this service.</param>
    public class MeterService(IMeterRepository meterRepository, IServiceOperationHandler serviceOperationHandler, ILogger<MeterService> logger) : IMeterService
    {
        private readonly IMeterRepository _meterRepository = meterRepository;
        private readonly IServiceOperationHandler _serviceOperationHandler = serviceOperationHandler;
        private readonly ILogger<MeterService> _logger = logger;

        /// <inheritdoc />
        public async Task<WMeter> CreateMeterAsync(WMeter meter)
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _meterRepository.AddAsync(meter);
            }, "Error creating meter.");
        }

        /// <inheritdoc />
        public async Task DeleteMeterAsync(int id)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _meterRepository.DeleteAsync(id);
            }, $"Error deleting meter with ID: {id}.");
        }

        /// <inheritdoc />
        public async Task<IEnumerable<WMeter>> GetAllMetersAsync()
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _meterRepository.GetAllAsync();
            }, "Error retrieving all meters.");
        }

        /// <inheritdoc />
        public async Task<WMeter> GetMeterByIdAsync(int id)
        {
            // Input validation: Ensure the ID is a positive integer.
            if (id <= 0)
            {
                _logger.LogError("Invalid Meter ID: {id}.", id);
                throw new InvalidInputException("Invalid Meter ID");
            }

            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _meterRepository.GetByIdAsync(id);
            }, $"Error retrieving meter with ID: {id}.");
        }

        /// <inheritdoc />
        public async Task<(List<WMeter>, int)> GetPaginatedMetersAsync(int pageNumber, int pageSize)
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _meterRepository.GetPaginatedMetersAsync(pageNumber, pageSize);
            }, $"Error retrieving paginated meters (page {pageNumber}, pageSize {pageSize}).");
        }

        /// <inheritdoc />
        public async Task UpdateMeterAsync(WMeter meter)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _meterRepository.UpdateAsync(meter);
            }, $"Error updating meter with ID: {meter.meterID}.");
        }
    }
}