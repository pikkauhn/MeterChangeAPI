using MeterChangeApi.Data;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeterChangeAPI.Repository
{
    public class AddressRepository : IAddressRepository, IRepository<Address>
    {
        private readonly ChangeOutContext _dbContext;

        public AddressRepository(ChangeOutContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Address entity)
        {
            _dbContext.Addresses.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var address = await _dbContext.Addresses.FindAsync(id);
            if (address != null)
            {
                _dbContext.Addresses.Remove(address);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            return await _dbContext.Addresses.ToListAsync();
        }

        public async Task<Address?> GetByIdAsync(int id)
        {
            return await _dbContext.Addresses.FindAsync(id);
        }

        public async Task UpdateAsync(Address entity)
        {
            _dbContext.Addresses.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Address>> GetAddressesByRangeAsync(double x, double y, double distanceInFeet)
        {
            string sql = @"
                        SELECT *
                        FROM Addresses
                        WHERE ST_Distance(POINT(Addresses.Location_Longitude, Addresses.Location_Latitude), POINT({0}, {1})) <= {2}";

            return await _dbContext.Addresses.FromSqlRaw(sql, x, y, distanceInFeet).ToListAsync();
        }

        public async Task<IEnumerable<Address>> GetAddressesByStreetAsync(string street)
        {
            return await _dbContext.Addresses
            .Where(a => a.Location_Address_Line1.Contains(street))
            .ToListAsync();
        }

        public async Task<Address?> GetAddressByLocationIcnAsync(int? locationIcn)
        {
            return await _dbContext.Addresses
            .FirstOrDefaultAsync(async => async.Location_ICN == locationIcn);
        }

        public async Task<IEnumerable<Address>> GetAddressesByBuildingStatusAsync(string buildingStatus)
        {
            return await _dbContext.Addresses
            .Where(a => a.Building_Status == buildingStatus)
            .ToListAsync();
        }

    }
}