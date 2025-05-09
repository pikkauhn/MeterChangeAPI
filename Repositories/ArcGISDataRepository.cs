using MeterChangeApi.Data;
using MeterChangeApi.Middleware.ExceptionHandling;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeterChangeApi.Repositories
{
    /// <summary>
    /// Implements the <see cref="IArcGISDataRepository"/> interface to provide data access
    /// for <see cref="ArcGISData"/> entities using Entity Framework Core.
    /// </summary>
    /// <param name="context">The database context for the application.</param>
    /// <param name="dbOperationHandler">The handler for executing database operations with error handling.</param>
    public class ArcGISDataRepository(ChangeOutContext context, IDatabaseOperationHandler dbOperationHandler) : IArcGISDataRepository
    {
        private readonly ChangeOutContext _context = context;
        private readonly IDatabaseOperationHandler _dbOperationHandler = dbOperationHandler;

        /// <inheritdoc />
        public async Task<ArcGISData> AddAsync(ArcGISData arcGISData)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                _context.ArcGISData.Add(arcGISData);
                await _context.SaveChangesAsync();
                return arcGISData;
            }, "Error adding ArcGIS data.");
        }

        /// <inheritdoc />
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
                // If not found, no exception is thrown as per the interface contract for DeleteAsync.
            }, $"Error deleting ArcGIS data with ID {id}.");
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ArcGISData>> GetAllAsync()
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                return await _context.ArcGISData
                    .Include(a => a.WEndpoint)
                    .ToListAsync();
            }, "Error retrieving all ArcGIS data.");
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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