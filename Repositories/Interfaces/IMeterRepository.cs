using MeterChangeApi.Models;

namespace MeterChangeApi.Repositories.Interfaces
{
    public interface IMeterRepository
    {
        Task<Wmeter> GetByIdAsync(int id);
        Task<IEnumerable<Wmeter>> GetAllAsync();
        Task<(List<Wmeter>, int)> GetPaginatedMetersAsync(int pageNumber, int pageSize);
        Task<Wmeter> AddAsync(Wmeter meter);
        Task UpdateAsync(Wmeter meter);
        Task DeleteAsync(int id);
    }
}
