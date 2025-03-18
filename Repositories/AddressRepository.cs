using MeterChangeApi.Data;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeterChangeApi.Repositories
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
            try
            {
                _dbContext.Addresses.Add(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var address = await _dbContext.Addresses.FindAsync(id);
                if (address != null)
                {
                    _dbContext.Addresses.Remove(address);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            try
            {
                return await _dbContext.Addresses.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Address?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbContext.Addresses.FindAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByIdAsync: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(Address entity)
        {
            try
            {
                _dbContext.Addresses.Update(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Address>> GetAddressesByRangeAsync(double x, double y, double distanceInFeet)
        {
            try
            {
                string sql = @"
                        SELECT *
                        FROM Addresses
                        WHERE ST_Distance(POINT(Addresses.Location_Longitude, Addresses.Location_Latitude), POINT({0}, {1})) <= {2}";

                return await _dbContext.Addresses.FromSqlRaw(sql, x, y, distanceInFeet).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAddressesByRangeAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Address>> GetAddressesByStreetAsync(string street)
        {
            try
            {
            return await _dbContext.Addresses
            .Where(a => a.Location_Address_Line1.Contains(street))
            .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAddressesByStreetAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<(List<Address>, int)> GetPaginatedAddressesAsync(int pageNumber, int pageSize)
        {
            try
            {
            var offset = (pageNumber - 1) * pageSize;

            var addresses = await _dbContext.Addresses
            .Skip(offset)
            .Take(pageSize)
            .ToListAsync();

            var totalCount = await _dbContext.Addresses.CountAsync();
            return (addresses, totalCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPaginatedAddressesAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Address?> GetAddressByLocationIcnAsync(int? locationIcn)
        {
            try
            {
            return await _dbContext.Addresses
            .FirstOrDefaultAsync(async => async.Location_ICN == locationIcn);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAddressByLocationIcnAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Address>> GetAddressesByBuildingStatusAsync(string buildingStatus)
        {
            try
            {
            return await _dbContext.Addresses
            .Where(a => a.Building_Status == buildingStatus)
            .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAddressesByBuildingStatusAsync: {ex.Message}");
                throw;
            }
        }

    }
}