using MeterChangeApi.Data;
using MeterChangeApi.Middleware.ExceptionHandling;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeterChangeApi.Repositories
{
    public class MeterRepository : IMeterRepository
    {
        private readonly ChangeOutContext _context;
        private readonly IDatabaseOperationHandler _dbOperationHandler;

        public MeterRepository(ChangeOutContext context, IDatabaseOperationHandler dbOperationHandler)
        {
            _context = context;
            _dbOperationHandler = dbOperationHandler;
        }

        public async Task<Wmeter> GetByIdAsync(int id)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var meter = await _context.meters
                    .Include(m => m.Address)
                    .Include(m => m.WEndpoint)
                    .FirstOrDefaultAsync(m => m.meterID == id);

                if (meter == null)
                {
                    throw new NotFoundException($"Meter with ID {id} not found.");
                }

                return meter;
            }, $"Error retrieving meter with ID {id}.");
        }

        public async Task<IEnumerable<Wmeter>> GetAllAsync()
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                return await _context.meters
                    .Include(m => m.Address)
                    .Include(m => m.WEndpoint)
                    .ToListAsync();
            }, "Error retrieving all meters.");
        }

        public async Task<(List<Wmeter>, int)> GetPaginatedMetersAsync(int pageNumber, int pageSize)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var offset = (pageNumber - 1) * pageSize;

                var meters = await _context.meters
                    .Include(m => m.Address)
                    .Include(m => m.WEndpoint)
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                var totalCount = await _context.meters.CountAsync();
                return (meters, totalCount);
            }, $"Error retrieving paginated meters (page:{pageNumber}, pageSize:{pageSize}).");
        }

        public async Task<Wmeter> AddAsync(Wmeter meter)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                _context.meters.Add(meter);
                await _context.SaveChangesAsync();
                return meter;
            }, "Error adding meter.");
        }

        public async Task UpdateAsync(Wmeter meter)
        {
            await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                _context.Entry(meter).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }, $"Error updating meter with ID {meter.meterID}.");
        }

        public async Task DeleteAsync(int id)
        {
            await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var meter = await _context.meters.FindAsync(id);
                if (meter == null)
                {
                    throw new NotFoundException($"Meter with ID {id} not found.");
                }

                _context.meters.Remove(meter);
                await _context.SaveChangesAsync();
            }, $"Error deleting meter with ID {id}.");
        }
    }
}