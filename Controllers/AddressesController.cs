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
        public async Task<IActionResult> GetPaginatedAddresses(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Invalid page number or page size.");
            }

            var (addresses, totalCount) = await _addressService.GetPaginatedAddressesAsync(pageNumber, pageSize);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var result = new
            {
                Addresses = addresses,
                TotalCount = totalCount,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return Ok(result);

        }

        [HttpGet("export-json")]
        public async Task<IActionResult> ExportAddressesToJson()
        {
            var memoryStream = await _addressService.ExportAddressesToJsonAsync();

            return File(memoryStream, "application/json", "addresses.json");
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

        [HttpGet("getalladdresses")]
        public async Task<IActionResult> GetAddresses(int pageNumber, int pageSize)
        {
            return Ok(await _addressService.GetAllAddressesAsync());
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
            if (address == null)
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