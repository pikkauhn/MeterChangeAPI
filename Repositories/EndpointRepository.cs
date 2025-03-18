using MeterChangeApi.Data;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeterChangeApi.Repositories
{
    public class EndpointRepository : IEndpointRepository
    {
        private readonly ChangeOutContext _context;

        public EndpointRepository(ChangeOutContext context)
        {
            _context = context;
        }

        public async Task<WEndpoint?> GetByIdAsync(int id)
        {
            return await _context.endpoints
            .Include(e => e.meter)
            .Include(e => e.ArcGISData)
            .FirstOrDefaultAsync(e => e.EndpointID == id);
        }

        public async Task<IEnumerable<WEndpoint>> GetAllAsync()
        {
            return await _context.endpoints
            .Include(e => e.meter)
            .Include(e => e.ArcGISData)
            .ToListAsync();
        }

        public async Task<(List<WEndpoint>, int)> GetPaginatedEndpointsAsync(int pageNumber, int pageSize)
        {
            var offset = (pageNumber - 1) * pageSize;

            var endpoints = await _context.endpoints
            .Skip(offset)
            .Take(pageSize)
            .ToListAsync();

            var totalCount = await _context.endpoints.CountAsync();
            return (endpoints, totalCount);
        }

        public async Task<WEndpoint> AddAsync(WEndpoint endpoint)
        {
            _context.endpoints.Add(endpoint);
            await _context.SaveChangesAsync();
            return endpoint;
        }

        public async Task UpdateAsync(WEndpoint endpoint)
        {
            _context.Entry(endpoint).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var endpoint = await _context.endpoints.FindAsync(id);
            if (endpoint != null)
            {
                _context.endpoints.Remove(endpoint);
                await _context.SaveChangesAsync();
            }
        }
    }
}