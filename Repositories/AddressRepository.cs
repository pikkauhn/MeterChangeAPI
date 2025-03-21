using Microsoft.EntityFrameworkCore;

using MeterChangeApi.Data;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeApi.Middleware.ExceptionHandling;

namespace MeterChangeApi.Repositories
{
    public class AddressRepository : IAddressRepository, IRepository<Address>
    {
        private readonly ChangeOutContext _dbContext;
        private readonly IDatabaseOperationHandler _dbOperationHandler;

        public AddressRepository(ChangeOutContext dbContext, IDatabaseOperationHandler databaseOperationHandler)
        {
            _dbContext = dbContext;
            _dbOperationHandler = databaseOperationHandler;
        }

        public async Task AddAsync(Address entity)
        {
            await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                _dbContext.Addresses.Add(entity);
                await _dbContext.SaveChangesAsync();
            }, "Error adding address.");
        }

        public async Task DeleteAsync(int id)
        {
            await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var address = await _dbContext.Addresses.FindAsync(id) ?? throw new NotFoundException($"Address with ID {id} not found.");
                var meters = _dbContext.meters.Where(m => m.AddressID == id);
                foreach (var meter in meters)
                {
                    meter.AddressID = null;
                }
                await _dbContext.SaveChangesAsync();

                _dbContext.Addresses.Remove(address);
                await _dbContext.SaveChangesAsync();

            }, $"Error deleting address with ID {id}.");
        }

        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            await _dbContext.Addresses.ToListAsync(), "Error retrieving all addresses.");
        }

        public async Task<Address> GetByIdAsync(int id)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var address = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.AddressID == id) 
                ?? throw new NotFoundException($"Address with ID {id} not found.");

                return address;

            }, $"Error retrieving address with ID {id}.");
        }

        public async Task UpdateAsync(Address entity)
        {
            await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                _dbContext.Addresses.Update(entity);
                await _dbContext.SaveChangesAsync();
            }, $"Error updating address with ID {entity.AddressID}.");
        }

        public async Task<IEnumerable<Address>> GetAddressesByRangeAsync(double x, double y, double distanceInFeet)
        {
            string sql = @"
        SELECT *
        FROM Addresses
        WHERE ST_Distance(POINT(Addresses.Location_Longitude, Addresses.Location_Latitude), POINT({0}, {1})) <= {2}";

            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            await _dbContext.Addresses.FromSqlRaw(sql, x, y, distanceInFeet).ToListAsync(), $"Error retrieving addresses by range (x:{x}, y:{y}, distance:{distanceInFeet}).");
        }

        public async Task<IEnumerable<Address>> GetAddressesByStreetAsync(string street)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            await _dbContext.Addresses.Where(a => a.Location_Address_Line1.Contains(street)).ToListAsync(), $"Error retrieving addresses by street '{street}'.");
        }

        public async Task<(List<Address>, int)> GetPaginatedAddressesAsync(int pageNumber, int pageSize)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var offset = (pageNumber - 1) * pageSize;

                var addresses = await _dbContext.Addresses
                    .OrderBy(a => a.AddressID)
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                var totalCount = await _dbContext.Addresses.CountAsync();
                return (addresses, totalCount);
            }, $"Error retrieving paginated addresses (page:{pageNumber}, pageSize:{pageSize}).");
        }

        public async Task<Address> GetAddressByLocationIcnAsync(int locationIcn)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var address = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.Location_ICN == locationIcn) 
                ?? throw new NotFoundException($"Address with locationICN {locationIcn} not found.");

                return address;
            }, $"Error retrieving address with locationICN {locationIcn}.");
        }

        public async Task<IEnumerable<Address>> GetAddressesByBuildingStatusAsync(string buildingStatus)
        {
            return await _dbOperationHandler.ExecuteDbOperationAsync(async () =>
            {
                var addresses = await _dbContext.Addresses.Where(a => a.Building_Status == buildingStatus).ToListAsync()
                ?? throw new NotFoundException($"Addresses with buildingStatus {buildingStatus} not found.");
                                
                return addresses;
            }, $"Error retrieving addresses with buildingStatus {buildingStatus}.");
        }
    }
}