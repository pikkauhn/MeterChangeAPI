using MeterChangeApi.Models;

namespace MeterChangeApi.Repositories.Interfaces
{
    public interface IEndpointRepository
    {
        Task<WEndpoint> GetByIdAsync(int id);
        Task<IEnumerable<WEndpoint>> GetAllAsync();
        Task<(List<WEndpoint>, int)> GetPaginatedEndpointsAsync(int pageNumber, int pageSize);
        Task<WEndpoint> AddAsync(WEndpoint endpoint);
        Task UpdateAsync(WEndpoint endpoint);
        Task DeleteAsync(int id);
    }
}