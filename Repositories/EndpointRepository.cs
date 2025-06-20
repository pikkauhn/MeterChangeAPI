using MeterChangeApi.Data;
using MeterChangeApi.Middleware.ExceptionHandling;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeterChangeApi.Repositories
{
    /// <summary>
    /// Implements the <see cref="IEndpointRepository"/> interface to provide data access
    /// for <see cref="WEndpoint"/> entities using Entity Framework Core.
    /// </summary>
    /// <param name="context">The database context for the application.</param>
    /// <param name="dbOperationHandler">The handler for executing database operations with error handling.</param>
    public class EndpointRepository(ChangeOutContext context, IDatabaseOperationHandler dbOperationHandler) : IEndpointRepository
    {
        private readonly ChangeOutContext _context = context;
        private readonly IDatabaseOperationHandler _dbOperationHandler = dbOperationHandler;

        /// <inheritdoc />
        public async Task<WEndpoint> GetByIdAsync(int id)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var endpoint = await _context.Endpoints
                    .Include(e => e.meter)
                    .Include(e => e.ArcGISData)
                    .FirstOrDefaultAsync(e => e.EndpointID == id)
                    ?? throw new NotFoundException($"Endpoint with ID {id} not found.");

                return endpoint;
            }, $"Error retrieving endpoint with ID {id}.");
        }

        /// <inheritdoc />
        public async Task<IEnumerable<WEndpoint>> GetAllAsync()
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                return await _context.Endpoints
                    .Include(e => e.meter)
                    .Include(e => e.ArcGISData)
                    .ToListAsync();
            }, "Error retrieving all endpoints.");
        }

        /// <inheritdoc />
        public async Task<(List<WEndpoint>, int)> GetPaginatedEndpointsAsync(int pageNumber, int pageSize)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var offset = (pageNumber - 1) * pageSize;

                var endpoints = await _context.Endpoints
                    .Include(e => e.meter)
                    .Include(e => e.ArcGISData)
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                var totalCount = await _context.Endpoints.CountAsync();
                return (endpoints, totalCount);
            }, $"Error retrieving paginated endpoints (page:{pageNumber}, pageSize:{pageSize}).");
        }

        /// <inheritdoc />
        public async Task<WEndpoint> AddAsync(WEndpoint endpoint)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                _context.Endpoints.Add(endpoint);
                await _context.SaveChangesAsync();
                return endpoint;
            }, "Error adding endpoint.");
        }

        /// <inheritdoc />
        public async Task UpdateAsync(WEndpoint endpoint)
        {
            await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                _context.Entry(endpoint).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }, $"Error updating endpoint with ID {endpoint.EndpointID}.");
        }

        /// <inheritdoc />
        public async Task DeleteAsync(int id)
        {
            await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var endpoint = await _context.Endpoints.FindAsync(id) ?? throw new NotFoundException($"Endpoint with ID {id} not found.");
                _context.Endpoints.Remove(endpoint);
                await _context.SaveChangesAsync();
            }, $"Error deleting endpoint with ID {id}.");
        }
    }
}