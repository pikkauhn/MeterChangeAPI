using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeApi.Services.Interfaces;

namespace MeterChangeApi.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task AddAddressAsync(Address address)
        {
            await _addressRepository.AddAsync(address);
        }

        public async Task DeleteAddressAsync(int id)
        {
            await _addressRepository.DeleteAsync(id);
        }

        public async Task<Address?> GetAddressByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid Address ID");
            }

            var address = await _addressRepository.GetByIdAsync(id);
            if (address == null)
            {
                throw new KeyNotFoundException($"Address with ID {id} not found.");
            }
            return address;
        }

        public async Task<Address?> GetAddressByLocationIcnAsync(int? locationIcn)
        {
            return await _addressRepository.GetAddressByLocationIcnAsync(locationIcn);
        }

        public async Task<IEnumerable<Address>> GetAddressesByBuildingStatusAsync(string buildingStatus)
        {
            return await _addressRepository.GetAddressesByBuildingStatusAsync(buildingStatus);
        }

        public async Task<IEnumerable<Address>> GetAddressesByRangeAsync(double x, double y, double distanceInFeet)
        {
            return await _addressRepository.GetAddressesByRangeAsync(x, y, distanceInFeet);
        }

        public async Task<IEnumerable<Address>> GetAddressesByStreetAsync(string street)
        {
            return await _addressRepository.GetAddressesByStreetAsync(street);
        }

        public async Task<IEnumerable<Address>> GetAllAddressesAsync()
        {
            return await _addressRepository.GetAllAsync();
        }

        public async Task<(List<Address>, int)> GetPaginatedAddressesAsync(int pageNumber, int pageSize)
        {
            return await _addressRepository.GetPaginatedAddressesAsync(pageNumber, pageSize);
        }

        public async Task<MemoryStream> ExportAddressesToJsonAsync()
        {
            var addresses = await _addressRepository.GetAllAsync();

            var jsonOptions = new JsonSerializerOptions { WriteIndented = true};
            var json = JsonSerializer.Serialize(addresses, jsonOptions);

            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            await writer.WriteAsync(json);
            await writer.FlushAsync();
            memoryStream.Position = 0;

            return memoryStream;
        }

        public async Task UpdateAddressAsync(Address address)
        {
            await _addressRepository.UpdateAsync(address);
        }
    }
}