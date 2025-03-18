using MeterChangeApi.Data;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeterChangeApi.Repositories
{
    public class MeterRepository : IMeterRepository
    {
        private readonly ChangeOutContext _context;

        public MeterRepository(ChangeOutContext context)
        {
            _context = context;
        }

        public async Task<Wmeter?> GetByIdAsync(int id)
        {
            return await _context.meters
            .Include(m => m.Address)
            .Include(m => m.WEndpoint)
            .FirstOrDefaultAsync(m => m.meterID == id);
        }

        public async Task<IEnumerable<Wmeter>> GetAllAsync()
        {
            return await _context.meters
            .Include(m => m.Address)
            .Include(m => m.WEndpoint)
            .ToListAsync();
        }

        public async Task<(List<Wmeter>, int)> GetPaginatedMetersAsync(int pageNumber, int pageSize)
        {
            var offset = (pageNumber - 1) * pageSize;

            var meters = await _context.meters                     
            .Skip(offset)
            .Take(pageSize)
            .ToListAsync();

            var totalCount = await _context.meters.CountAsync();
            return (meters, totalCount);
        }

        public async Task<Wmeter> AddAsync(Wmeter meter)
        {
            _context.meters.Add(meter);
            await _context.SaveChangesAsync();
            return meter;
        }

        public async Task UpdateAsync(Wmeter meter)
        {
            _context.Entry(meter).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var meter = await _context.meters.FindAsync(id);
            if (meter != null)
            {
                _context.meters.Remove(meter);
                await _context.SaveChangesAsync();
            }
        }
    }
}