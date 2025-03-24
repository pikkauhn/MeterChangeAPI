using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Middleware.ExceptionHandling;

namespace MeterChangeApi.Services
{
    public class EndpointService : IEndpointService
    {
        private readonly IEndpointRepository _endpointRepository;
        private readonly IServiceOperationHandler _serviceOperationHandler;
        private readonly ILogger<EndpointService> _logger;

        public EndpointService(IEndpointRepository endpointRepository, IServiceOperationHandler serviceOperationHandler, ILogger<EndpointService> logger)
        {
            _endpointRepository = endpointRepository;
            _serviceOperationHandler = serviceOperationHandler;
            _logger = logger;
        }

        public async Task<WEndpoint> GetEndpointByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"Invalid Endpoint ID: {id}.");
                throw new InvalidInputException("Invalid Endpoint ID");
            }

            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _endpointRepository.GetByIdAsync(id);
            }, $"Error retrieving endpoint with ID: {id}.");
        }

        public async Task<IEnumerable<WEndpoint>> GetAllEndpointsAsync()
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _endpointRepository.GetAllAsync();
            }, "Error retrieving all endpoints.");
        }

        public async Task<(List<WEndpoint>, int)> GetPaginatedEndpointsAsync(int pageNumber, int pageSize)
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _endpointRepository.GetPaginatedEndpointsAsync(pageNumber, pageSize);
            }, $"Error retrieving paginated endpoints (page {pageNumber}, pageSize {pageSize}).");
        }

        public async Task<WEndpoint> CreateEndpointAsync(WEndpoint endpoint)
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _endpointRepository.AddAsync(endpoint);
            }, "Error creating endpoint.");
        }

        public async Task UpdateEndpointAsync(WEndpoint endpoint)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _endpointRepository.UpdateAsync(endpoint);
            }, $"Error updating endpoint with ID: {endpoint.EndpointID}.");
        }

        public async Task DeleteEndpointAsync(int id)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _endpointRepository.DeleteAsync(id);
            }, $"Error deleting endpoint with ID: {id}.");
        }
    }
}