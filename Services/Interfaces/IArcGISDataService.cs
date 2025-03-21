using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeterChangeApi.Models;

namespace MeterChangeApi.Services.Interfaces
{
    public interface IArcGISDataService
    {
        Task<ArcGISData> GetArcGISDataByIdAsync(int id);
        Task<IEnumerable<ArcGISData>> GetAllArcGISDataAsync();
        Task<(List<ArcGISData>, int)> GetPaginatedArcGISDataAsync(int pageNumber, int pageSize);
        Task<ArcGISData> CreateArcGISDataAsync(ArcGISData arcGISData);
        Task UpdateArcGISDataAsync(ArcGISData arcGISData);
        Task DeleteArcGISDataAsync(int id);
    }
}