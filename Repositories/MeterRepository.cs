using MeterChangeApi.Data;
using MeterChangeApi.Middleware.ExceptionHandling;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeterChangeApi.Repositories
{
    /// <summary>
    /// Implements the <see cref="IMeterRepository"/> interface to provide data access
    /// for <see cref="WMeter"/> entities using Entity Framework Core.
    /// </summary>
    /// <param name="context">The database context for the application.</param>
    /// <param name="dbOperationHandler">The handler for executing database operations with error handling.</param>
    public class MeterRepository(ChangeOutContext context, IDatabaseOperationHandler dbOperationHandler) : IMeterRepository
    {
        private readonly ChangeOutContext _context = context;
        private readonly IDatabaseOperationHandler _dbOperationHandler = dbOperationHandler;

        /// <inheritdoc />
        public async Task<WMeter> GetByIdAsync(int id)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var meter = await _context.Meters
                    .Include(m => m.Address)
                    .Include(m => m.WEndpoint)
                    .FirstOrDefaultAsync(m => m.meterID == id) ?? throw new NotFoundException($"Meter with ID {id} not found.");
                return meter;
            }, $"Error retrieving meter with ID {id}.");
        }

        /// <inheritdoc />
        public async Task<IEnumerable<WMeter>> GetAllAsync()
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                return await _context.Meters
                    .Include(m => m.Address)
                    .Include(m => m.WEndpoint)
                    .ToListAsync();
            }, "Error retrieving all meters.");
        }

        /// <inheritdoc />
        public async Task<(List<WMeter>, int)> GetPaginatedMetersAsync(int pageNumber, int pageSize)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var offset = (pageNumber - 1) * pageSize;

                var meters = await _context.Meters
                    .Include(m => m.Address)
                    .Include(m => m.WEndpoint)
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                var totalCount = await _context.Meters.CountAsync();
                return (meters, totalCount);
            }, $"Error retrieving paginated meters (page:{pageNumber}, pageSize:{pageSize}).");
        }

        /// <inheritdoc />
        public async Task<WMeter> AddAsync(WMeter meter)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                _context.Meters.Add(meter);
                await _context.SaveChangesAsync();
                return meter;
            }, "Error adding meter.");
        }

        /// <inheritdoc />
        public async Task UpdateAsync(WMeter meter)
        {
            await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                _context.Entry(meter).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }, $"Error updating meter with ID {meter.meterID}.");
        }

        /// <inheritdoc />
        public async Task DeleteAsync(int id)
        {
            await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var meter = await _context.Meters.FindAsync(id) ?? throw new NotFoundException($"Meter with ID {id} not found.");
                _context.Meters.Remove(meter);
                await _context.SaveChangesAsync();
            }, $"Error deleting meter with ID {id}.");
        }
    }
}