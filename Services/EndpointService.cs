using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeApi.Services.Interfaces;

namespace MeterChangeApi.Services
{
    public class EndpointService : IEndpointService
    {
        private readonly IEndpointRepository _endpointRepository;

        public EndpointService(IEndpointRepository endpointRepository)
        {
            _endpointRepository = endpointRepository;
        }

        public async Task<WEndpoint?> GetEndpointByIdAsync(int id)
        {          
            return await _endpointRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<WEndpoint>> GetAllEndpointsAsync()
        {
            return await _endpointRepository.GetAllAsync();
        }

        public async Task<(List<WEndpoint>, int)> GetPaginatedEndpointsAsync(int pageNumber, int pageSize)
        {
            return await _endpointRepository.GetPaginatedEndpointsAsync(pageNumber, pageSize);
        }

        public async Task<WEndpoint> CreateEndpointAsync(WEndpoint endpoint)
        {
            return await _endpointRepository.AddAsync(endpoint);
        }

        public async Task UpdateEndpointAsync(WEndpoint endpoint)
        {
            await _endpointRepository.UpdateAsync(endpoint);
        }

        public async Task DeleteEndpointAsync(int id)
        {
            await _endpointRepository.DeleteAsync(id);
        }
    }
}