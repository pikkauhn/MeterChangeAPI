using Microsoft.AspNetCore.Mvc;
using MeterChangeApi.Models;
using MeterChangeApi.Services.Interfaces;

namespace MeterChangeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            var address = await _addressService.GetAddressByIdAsync(id);
            if (address == null)
            {
                return NotFound();
            }
            return address;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
        {
            return Ok(await _addressService.GetAllAddressesAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Address>> PostAddress(Address address)
        {
            await _addressService.AddAddressAsync(address);
            return CreatedAtAction(nameof(GetAddress), new { id = address.AddressID }, address);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(int id, Address address)
        {
            if (id != address.AddressID)
            {
                return BadRequest();
            }

            await _addressService.UpdateAddressAsync(address);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            await _addressService.DeleteAddressAsync(id);
            return NoContent();
        }

        [HttpGet("range")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddressesByRange(double x, double y, double distanceInFeet)
        {
            return Ok(await _addressService.GetAddressesByRangeAsync(x, y, distanceInFeet));
        }

        [HttpGet("street/{street}")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddressesByStreet(string street)
        {
            return Ok(await _addressService.GetAddressesByStreetAsync(street));
        }

        [HttpGet("locationicn/{locationIcn}")]
        public async Task<ActionResult<Address>> GetAddressByLocationIcn(int? locationIcn)
        {
            var address = await _addressService.GetAddressByLocationIcnAsync(locationIcn);
            if(address == null)
            {
                return NotFound();
            }
            return Ok(address);
        }

        [HttpGet("buildingstatus/{buildingStatus}")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddressesByBuildingStatus(string buildingStatus)
        {
            return Ok(await _addressService.GetAddressesByBuildingStatusAsync(buildingStatus));
        }
    }
}