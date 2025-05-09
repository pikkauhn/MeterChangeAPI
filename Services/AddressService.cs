using System.Text.Json;

using MeterChangeApi.Models;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeApi.Middleware.ExceptionHandling;
using MeterChangeApi.Options.Config;
using Microsoft.Extensions.Logging;

namespace MeterChangeApi.Services
{
    /// <summary>
    /// Implements the <see cref="IAddressService"/> interface to handle operations related to addresses.
    /// This service interacts with the <see cref="IAddressRepository"/> for data access and uses the
    /// <see cref="IServiceOperationHandler"/> for consistent error handling and logging.
    /// </summary>
    /// <param name="addressRepository">The repository for accessing address data.</param>
    /// <param name="logger">The logger instance for logging messages within this service.</param>
    /// <param name="serviceOperationHandler">The handler for executing service operations with error handling.</param>
    public class AddressService(IAddressRepository addressRepository, ILogger<AddressService> logger, IServiceOperationHandler serviceOperationHandler) : IAddressService
    {
        private readonly IAddressRepository _addressRepository = addressRepository;
        private readonly ILogger<AddressService> _logger = logger;
        private readonly IServiceOperationHandler _serviceOperationHandler = serviceOperationHandler;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

        /// <inheritdoc />
        public async Task AddAddressAsync(Address address)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _addressRepository.AddAsync(address);
            }, "Error adding address.");
        }

        /// <inheritdoc />
        public async Task DeleteAddressAsync(int id)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _addressRepository.DeleteAsync(id);
            }, $"Error deleting address with ID: {id}.");
        }

        /// <inheritdoc />
        public async Task<Address> GetAddressByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Invalid Address ID: {id}.", id);
                throw new InvalidInputException("Invalid Address ID");
            }

            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _addressRepository.GetByIdAsync(id);
            }, $"Error getting address by ID: {id}.");
        }

        /// <inheritdoc />
        public async Task<Address> GetAddressByLocationIcnAsync(int locationIcn)
        {
            if (locationIcn <= 0)
            {
                _logger.LogError("Invalid locationICN: {locationIcn}.", locationIcn);
                throw new InvalidInputException("Invalid locationICN");
            }

            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _addressRepository.GetAddressByLocationIcnAsync(locationIcn);
            }, $"Error getting address by locationICN: {locationIcn}.");
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<IEnumerable<Address>> GetAddressesByRangeAsync(double x, double y, double distanceInFeet)
        {
            if (distanceInFeet <= 0)
            {
                _logger.LogError("Invalid distanceInFeet: must be greater than zero.");
                throw new InvalidInputException("Invalid distanceInFeet");
            }

            if (x < CountyCoordinates.MinX || x > CountyCoordinates.MaxX || y < CountyCoordinates.MinY || y > CountyCoordinates.MaxY)
            {
                _logger.LogError("Coordinates (x={x}, y={y}) are outside the county's range.", x, y);
                throw new InvalidInputException("Coordinates are outside the county's range.");
            }

            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _addressRepository.GetAddressesByRangeAsync(x, y, distanceInFeet);
            }, $"Error getting addresses by range: x={x}, y={y}, distance={distanceInFeet}.");
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<IEnumerable<Address>> GetAllAddressesAsync()
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _addressRepository.GetAllAsync();
            }, "Error getting all addresses.");
        }

        /// <inheritdoc />
        public async Task<(List<Address>, int)> GetPaginatedAddressesAsync(int pageNumber, int pageSize)
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                return await _addressRepository.GetPaginatedAddressesAsync(pageNumber, pageSize);
            }, $"Error getting paginated addresses: pageNumber={pageNumber}, pageSize={pageSize}.");
        }

        /// <inheritdoc />
        public async Task<MemoryStream> ExportAddressesToJsonAsync()
        {
            return await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                var addresses = await _addressRepository.GetAllAsync();
                var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(addresses, _jsonSerializerOptions);
                return new MemoryStream(jsonBytes);
            }, "Error exporting addresses to JSON.");
        }

        /// <inheritdoc />
        public async Task UpdateAddressAsync(Address address)
        {
            await _serviceOperationHandler.ExecuteServiceOperationAsync(async () =>
            {
                await _addressRepository.UpdateAsync(address);
            }, $"Error updating address.");
        }
    }
}