using MeterChangeApi.Data;
using MeterChangeApi.Models;
 using MeterChangeAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeterChangeAPI.Repositories
{
    public class ArcGISDataRepository : IArcGISDataRepository
    {
        private readonly ChangeOutContext _context;

        public ArcGISDataRepository(ChangeOutContext context)
        {
            _context = context;
        }

        public async Task<ArcGISData> AddAsync(ArcGISData arcGISData)
        {
            _context.ArcGISData.Add(arcGISData);;
            await _context.SaveChangesAsync();
            return arcGISData;
        }

        public async Task DeleteAsync(int id)
        {
            var arcGISData = await _context.ArcGISData.FindAsync(id);
            if (arcGISData != null)
            {
                _context.ArcGISData.Remove(arcGISData);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ArcGISData>> GetAllAsync()
        {
            return await _context.ArcGISData
            .Include(a => a.WEndpoint)
            .ToListAsync();
        }

        public async Task<ArcGISData?> GetByIdAsync(int id)
        {
            return await _context.ArcGISData
            .Include(a => a.WEndpoint)
            .FirstOrDefaultAsync(a => a.ArcGISDataID == id);
        }

        public async Task<(List<ArcGISData>, int)> GetPaginatedArcGISDataAsync(int pageNumber, int pageSize)        
            {
            var offset = (pageNumber - 1) * pageSize;

            var arcGISDatas = await _context.ArcGISData
            .Skip(offset)
            .Take(pageSize)
            .ToListAsync();

            var totalCount = await _context.ArcGISData.CountAsync();
            return (arcGISDatas, totalCount);
        }

        public async Task UpdateAsync(ArcGISData arcGISData)
        {
            _context.Entry(arcGISData).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}