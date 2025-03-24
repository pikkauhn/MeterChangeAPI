using Microsoft.EntityFrameworkCore;

using MeterChangeApi.Data;
using MeterChangeApi.Middleware.ExceptionHandling;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;

namespace MeterChangeApi.Repositories
{
    public class EndpointRepository : IEndpointRepository
    {
        private readonly ChangeOutContext _context;
        private readonly IDatabaseOperationHandler _dbOperationHandler;

        public EndpointRepository(ChangeOutContext context, IDatabaseOperationHandler dbOperationHandler)
        {
            _context = context;
            _dbOperationHandler = dbOperationHandler;
        }

        public async Task<WEndpoint> GetByIdAsync(int id)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var endpoint = await _context.endpoints
                    .Include(e => e.meter)
                    .Include(e => e.ArcGISData)
                    .FirstOrDefaultAsync(e => e.EndpointID == id)
                    ?? throw new NotFoundException($"Endpoint with ID {id} not found.");

                return endpoint;
            }, $"Error retrieving endpoint with ID {id}.");
        }

        public async Task<IEnumerable<WEndpoint>> GetAllAsync()
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                return await _context.endpoints
                    .Include(e => e.meter)
                    .Include(e => e.ArcGISData)
                    .ToListAsync();
            }, "Error retrieving all endpoints.");
        }

        public async Task<(List<WEndpoint>, int)> GetPaginatedEndpointsAsync(int pageNumber, int pageSize)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var offset = (pageNumber - 1) * pageSize;

                var endpoints = await _context.endpoints
                    .Include(e => e.meter)
                    .Include(e => e.ArcGISData)
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                var totalCount = await _context.endpoints.CountAsync();
                return (endpoints, totalCount);
            }, $"Error retrieving paginated endpoints (page:{pageNumber}, pageSize:{pageSize}).");
        }

        public async Task<WEndpoint> AddAsync(WEndpoint endpoint)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                _context.endpoints.Add(endpoint);
                await _context.SaveChangesAsync();
                return endpoint;
            }, "Error adding endpoint.");
        }

        public async Task UpdateAsync(WEndpoint endpoint)
        {
            await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                _context.Entry(endpoint).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }, $"Error updating endpoint with ID {endpoint.EndpointID}.");
        }

        public async Task DeleteAsync(int id)
        {
            await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var endpoint = await _context.endpoints.FindAsync(id);
                if (endpoint == null)
                {
                    throw new NotFoundException($"Endpoint with ID {id} not found.");
                }

                _context.endpoints.Remove(endpoint);
                await _context.SaveChangesAsync();
            }, $"Error deleting endpoint with ID {id}.");
        }
    }
}