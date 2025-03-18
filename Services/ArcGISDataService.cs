using MeterChangeApi.Models;
using MeterChangeAPI.Repositories.Interfaces;
using MeterChangeAPI.Services.Interfaces;

namespace MeterChangeAPI.Services
{
    public class ArcGISDataService : IArcGISDataService
    {
        private readonly IArcGISDataRepository _arcGISDataRepository;
        public ArcGISDataService(IArcGISDataRepository arcGISDataRepository)
        {
            _arcGISDataRepository = arcGISDataRepository;
        }
        public async Task<ArcGISData> CreateArcGISDataAsync(ArcGISData arcGISData)
        {
            return await _arcGISDataRepository.AddAsync(arcGISData);
        }

        public async Task DeleteArcGISDataAsync(int id)
        {
            await _arcGISDataRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ArcGISData>> GetAllArcGISDataAsync()
        {
            return await _arcGISDataRepository.GetAllAsync();
        }

        public async Task<ArcGISData?> GetArcGISDataByIdAsync(int id)
        {
            return await _arcGISDataRepository.GetByIdAsync(id);
        }

        public async Task<(List<ArcGISData>, int)> GetPaginatedArcGISDataAsync(int pageNumber, int pageSize)
        {
            return await _arcGISDataRepository.GetPaginatedArcGISDataAsync(pageNumber, pageSize);
        }

        public async Task UpdateArcGISDataAsync(ArcGISData arcGISData)
        {
            await _arcGISDataRepository.UpdateAsync(arcGISData);
        }
    }
}