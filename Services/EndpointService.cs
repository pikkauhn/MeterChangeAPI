using MeterChangeApi.Middleware.ExceptionHandling;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeApi.Services.Interfaces;

namespace MeterChangeApi.Services
{
    /// <summary>
    /// Implements the <see cref="IEndpointService"/> interface to handle operations related to water endpoints.
    /// This service interacts with the <see cref="IEndpointRepository"/> for data access and uses the
    /// <see cref="IServiceOperationHandler"/> for consistent error handling and logging.
    /// </summary>
    /// <param name="endpointRepository">The repository for accessing water endpoint data.</param>
    /// <param name="serviceOperationHandler">The handler for executing service operations with error handling.</param>
    /// <param name="logger">The logger instance for logging messages within this service.</param>
    public class EndpointService(IEndpointRepository endpointRepository, IServiceOperationHandler serviceOperationHandler, ILogger<EndpointService> logger) : IEndpointService
    {
        private readonly IEndpointRepository _endpointRepository = endpointRepository;
        private readonly IServiceOperationHandler _serviceOperationHandler = serviceOperationHandler;
        private readonly ILogger<EndpointService> _logger = logger;

        /// <inheritdoc />
        public async Task<WEndpoint> GetEndpointByIdAsync(int id)
        {
            // Input validation: Ensure the ID is a positive integer.
            if (id <= 0)
            {
                _logger.LogError("Invalid Endpoint ID: {id}.", id);
                throw new InvalidInputException("Invalid Endpoint ID");
            }

            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _endpointRepository.GetByIdAsync(id);
            }, $"Error retrieving endpoint with ID: {id}.");
        }

        /// <inheritdoc />
        public async Task<IEnumerable<WEndpoint>> GetAllEndpointsAsync()
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _endpointRepository.GetAllAsync();
            }, "Error retrieving all endpoints.");
        }

        /// <inheritdoc />
        public async Task<(List<WEndpoint>, int)> GetPaginatedEndpointsAsync(int pageNumber, int pageSize)
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _endpointRepository.GetPaginatedEndpointsAsync(pageNumber, pageSize);
            }, $"Error retrieving paginated endpoints (page {pageNumber}, pageSize {pageSize}).");
        }

        /// <inheritdoc />
        public async Task<WEndpoint> CreateEndpointAsync(WEndpoint endpoint)
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _endpointRepository.AddAsync(endpoint);
            }, "Error creating endpoint.");
        }

        /// <inheritdoc />
        public async Task UpdateEndpointAsync(WEndpoint endpoint)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _endpointRepository.UpdateAsync(endpoint);
            }, $"Error updating endpoint with ID: {endpoint.EndpointID}.");
        }

        /// <inheritdoc />
        public async Task DeleteEndpointAsync(int id)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _endpointRepository.DeleteAsync(id);
            }, $"Error deleting endpoint with ID: {id}.");
        }
    }
}