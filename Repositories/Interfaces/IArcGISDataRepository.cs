using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeterChangeApi.Models;

namespace MeterChangeApi.Repositories.Interfaces
{
    public interface IArcGISDataRepository
    {
        Task<ArcGISData> GetByIdAsync(int id);
        Task<IEnumerable<ArcGISData>> GetAllAsync();
        Task<(List<ArcGISData>, int)> GetPaginatedArcGISDataAsync(int pageNumber, int pageSize);
        Task<ArcGISData> AddAsync(ArcGISData arcGISData);
        Task UpdateAsync(ArcGISData arcGISData);
        Task DeleteAsync(int id);
    }
}