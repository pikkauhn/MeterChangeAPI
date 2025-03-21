using MeterChangeApi.Data;
using MeterChangeApi.Middleware.ExceptionHandling;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeterChangeApi.Repositories
{
    public class ArcGISDataRepository : IArcGISDataRepository
    {
        private readonly ChangeOutContext _context;
        private readonly IDatabaseOperationHandler _dbOperationHandler;

        public ArcGISDataRepository(ChangeOutContext context, IDatabaseOperationHandler dbOperationHandler)
        {
            _context = context;
            _dbOperationHandler = dbOperationHandler;
        }

        public async Task<ArcGISData> AddAsync(ArcGISData arcGISData)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                _context.ArcGISData.Add(arcGISData);
                await _context.SaveChangesAsync();
                return arcGISData;
            }, "Error adding ArcGIS data.");
        }

        public async Task DeleteAsync(int id)
        {
            await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var arcGISData = await _context.ArcGISData.FindAsync(id);
                if (arcGISData != null)
                {
                    _context.ArcGISData.Remove(arcGISData);
                    await _context.SaveChangesAsync();
                }
            }, $"Error deleting ArcGIS data with ID {id}.");
        }

        public async Task<IEnumerable<ArcGISData>> GetAllAsync()
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                return await _context.ArcGISData
                    .Include(a => a.WEndpoint)
                    .ToListAsync();
            }, "Error retrieving all ArcGIS data.");
        }

        public async Task<ArcGISData> GetByIdAsync(int id)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var arcGISData = await _context.ArcGISData
                .Include(a => a.WEndpoint)
                .FirstOrDefaultAsync(a => a.ArcGISDataID == id)
                ?? throw new NotFoundException($"ArcGISData with ID {id} not found.");

                return arcGISData;
            }, $"Error retrieving ArcGISData with ID {id}.");
        }

        public async Task<(List<ArcGISData>, int)> GetPaginatedArcGISDataAsync(int pageNumber, int pageSize)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var offset = (pageNumber - 1) * pageSize;

                var arcGISDatas = await _context.ArcGISData
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                var totalCount = await _context.ArcGISData.CountAsync();
                return (arcGISDatas, totalCount);
            }, $"Error retrieving paginated ArcGIS data (page:{pageNumber}, pageSize:{pageSize}).");
        }

        public async Task UpdateAsync(ArcGISData arcGISData)
        {
            await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                _context.Entry(arcGISData).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }, $"Error updating ArcGIS data with ID {arcGISData.ArcGISDataID}.");
        }
    }
}