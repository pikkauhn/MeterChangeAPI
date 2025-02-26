using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeApi.Services.Interfaces;

namespace MeterChangeAPI.Services
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
            return await _addressRepository.GetByIdAsync(id);
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

        public async Task UpdateAddressAsync(Address address)
        {
            await _addressRepository.UpdateAsync(address);
        }
    }
}