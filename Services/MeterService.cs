using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeApi.Services.Interfaces;

namespace MeterChangeApi.Services
{
    public class MeterService : IMeterService
    {
        private readonly IMeterRepository _meterRepository;

        public MeterService(IMeterRepository meterRepository)
        {
            _meterRepository = meterRepository;
        }

        public async Task<Wmeter> CreateMeterAsync(Wmeter meter)
        {
            return await _meterRepository.AddAsync(meter);
        }

        public async Task DeleteMeterAsync(int id)
        {
            await _meterRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Wmeter>> GetAllMetersAsync()
        {
            return await _meterRepository.GetAllAsync();
        }

        public async Task<Wmeter?> GetMeterByIdAsync(int id)
        {
            return await _meterRepository.GetByIdAsync(id);
        }

        public async Task<(List<Wmeter>, int)> GetPaginatedMetersAsync(int pageNumber, int pageSize)
        {
            return await _meterRepository.GetPaginatedMetersAsync(pageNumber, pageNumber);
        }

        public async Task UpdateMeterAsync(Wmeter meter)
        {
            await _meterRepository.UpdateAsync(meter);
        }
    }
}