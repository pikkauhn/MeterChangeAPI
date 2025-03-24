using System.Text.Json;

using MeterChangeApi.Models;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeApi.Middleware.ExceptionHandling;
using MeterChangeApi.Data.Config;

namespace MeterChangeApi.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ILogger<AddressService> _logger;
        private readonly IServiceOperationHandler _serviceOperationHandler;

        public AddressService(IAddressRepository addressRepository, ILogger<AddressService> logger, IServiceOperationHandler serviceOperationHandler)
        {
            _addressRepository = addressRepository;
            _logger = logger;
            _serviceOperationHandler = serviceOperationHandler;
        }

        public async Task AddAddressAsync(Address address)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _addressRepository.AddAsync(address);
            }, "Error adding address.");
        }

        public async Task DeleteAddressAsync(int id)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _addressRepository.DeleteAsync(id);
            }, $"Error deleting address with ID: {id}.");
        }

        public async Task<Address> GetAddressByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"Invalid Address ID: {id}.");
                throw new InvalidInputException("Invalid Address ID");
            }

            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _addressRepository.GetByIdAsync(id);
            }, $"Error getting address by ID: {id}.");
        }

        public async Task<Address> GetAddressByLocationIcnAsync(int locationIcn)
        {
            if (locationIcn <= 0)
            {
                _logger.LogError($"Invalid locationICN: {locationIcn}.");
                throw new InvalidInputException("Invalid locationICN");
            }

            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _addressRepository.GetAddressByLocationIcnAsync(locationIcn);
            }, $"Error getting address by locationICN: {locationIcn}.");
        }

        public async Task<IEnumerable<Address>> GetAddressesByBuildingStatusAsync(string buildingStatus)
        {
            if (string.IsNullOrEmpty(buildingStatus))
            {
                _logger.LogError("Invalid buildingStatus: null or empty.");
                throw new InvalidInputException("Invalid buildingStatus");
            }

            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _addressRepository.GetAddressesByBuildingStatusAsync(buildingStatus);
            }, $"Error getting addresses by buildingStatus: {buildingStatus}.");
        }

        public async Task<IEnumerable<Address>> GetAddressesByRangeAsync(double x, double y, double distanceInFeet)
        {

            if (distanceInFeet <= 0)
            {
                _logger.LogError("Invalid distanceInFeet: must be greater than zero.");
                throw new InvalidInputException("Invalid distanceInFeet");
            }

            if (x < CountyCoordinates.MinX || x > CountyCoordinates.MaxX || y < CountyCoordinates.MinY || y > CountyCoordinates.MaxY)
            {
                _logger.LogError($"Coordinates (x={x}, y={y}) are outside the county's range.");
                throw new InvalidInputException("Coordinates are outside the county's range.");
            }

            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _addressRepository.GetAddressesByRangeAsync(x, y, distanceInFeet);
            }, $"Error getting addresses by range: x={x}, y={y}, distance={distanceInFeet}.");
        }

        public async Task<IEnumerable<Address>> GetAddressesByStreetAsync(string street)
        {
            if (string.IsNullOrEmpty(street))
            {
                _logger.LogError("Invalid street: null or empty.");
                throw new InvalidInputException("Invalid street");
            }

            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _addressRepository.GetAddressesByStreetAsync(street);
            }, $"Error getting addresses by street: {street}.");
        }

        public async Task<IEnumerable<Address>> GetAllAddressesAsync()
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _addressRepository.GetAllAsync();
            }, "Error getting all addresses.");
        }

        public async Task<(List<Address>, int)> GetPaginatedAddressesAsync(int pageNumber, int pageSize)
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _addressRepository.GetPaginatedAddressesAsync(pageNumber, pageSize);
            }, $"Error getting paginated addresses: pageNumber={pageNumber}, pageSize={pageSize}.");
        }

        public async Task<MemoryStream> ExportAddressesToJsonAsync()
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                var addresses = await _addressRepository.GetAllAsync();
                var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(addresses, new JsonSerializerOptions { WriteIndented = true });
                return new MemoryStream(jsonBytes);
            }, "Error exporting addresses to JSON.");
        }

        public async Task UpdateAddressAsync(Address address)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _addressRepository.UpdateAsync(address);
            }, $"Error updating address.");
        }
    }
}