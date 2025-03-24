using MeterChangeApi.Models;

namespace MeterChangeApi.Services.Interfaces
{
    public interface IEndpointService
    {
        Task<WEndpoint> GetEndpointByIdAsync(int id);
        Task<IEnumerable<WEndpoint>> GetAllEndpointsAsync();
        Task<(List<WEndpoint>, int)> GetPaginatedEndpointsAsync(int pageNumber, int pageSize);
        Task<WEndpoint> CreateEndpointAsync(WEndpoint endpoint);
        Task UpdateEndpointAsync(WEndpoint endpoint);
        Task DeleteEndpointAsync(int id);
    }
}