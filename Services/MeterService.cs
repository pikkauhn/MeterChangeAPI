using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Middleware.ExceptionHandling;

namespace MeterChangeApi.Services
{
    public class MeterService : IMeterService
    {
        private readonly IMeterRepository _meterRepository;
        private readonly IServiceOperationHandler _serviceOperationHandler;
        private readonly ILogger<MeterService> _logger;

        public MeterService(IMeterRepository meterRepository, IServiceOperationHandler serviceOperationHandler, ILogger<MeterService> logger)
        {
            _meterRepository = meterRepository;
            _serviceOperationHandler = serviceOperationHandler;
            _logger = logger;
        }

        public async Task<Wmeter> CreateMeterAsync(Wmeter meter)
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _meterRepository.AddAsync(meter);
            }, "Error creating meter.");
        }

        public async Task DeleteMeterAsync(int id)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _meterRepository.DeleteAsync(id);
            }, $"Error deleting meter with ID: {id}.");
        }

        public async Task<IEnumerable<Wmeter>> GetAllMetersAsync()
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _meterRepository.GetAllAsync();
            }, "Error retrieving all meters.");
        }

        public async Task<Wmeter> GetMeterByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"Invalid Meter ID: {id}.");
                throw new InvalidInputException("Invalid Meter ID");
            }

            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _meterRepository.GetByIdAsync(id);
            }, $"Error retrieving meter with ID: {id}.");
        }

        public async Task<(List<Wmeter>, int)> GetPaginatedMetersAsync(int pageNumber, int pageSize)
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _meterRepository.GetPaginatedMetersAsync(pageNumber, pageSize);
            }, $"Error retrieving paginated meters (page {pageNumber}, pageSize {pageSize}).");
        }

        public async Task UpdateMeterAsync(Wmeter meter)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _meterRepository.UpdateAsync(meter);
            }, $"Error updating meter with ID: {meter.meterID}.");
        }
    }
}