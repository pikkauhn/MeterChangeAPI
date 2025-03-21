using MeterChangeApi.Models;

namespace MeterChangeApi.Services.Interfaces
{
    public interface IMeterService
    {
        Task<Wmeter> GetMeterByIdAsync(int id);
        Task<IEnumerable<Wmeter>> GetAllMetersAsync();
        Task<(List<Wmeter>, int)> GetPaginatedMetersAsync(int pageNumber, int pageSize);
        Task<Wmeter> CreateMeterAsync(Wmeter meter);
        Task UpdateMeterAsync(Wmeter meter);
        Task DeleteMeterAsync(int id);
    }
}