using Microsoft.AspNetCore.Mvc;
using MeterChangeApi.Models;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Middleware.ExceptionHandling;

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
            try
            {
                var address = await _addressService.GetAddressByIdAsync(id);
                return Ok(address);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidInputException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedAddresses(int pageNumber, int pageSize)
        {
            try
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
            catch (ServiceException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("export-json")]
        public async Task<IActionResult> ExportAddressesToJson()
        {
            try
            {
                var memoryStream = await _addressService.ExportAddressesToJsonAsync();
                return File(memoryStream, "application/json", "addresses.json");
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Address>> PostAddress(Address address)
        {
            try
            {
                await _addressService.AddAddressAsync(address);
                return CreatedAtAction(nameof(GetAddress), new { id = address.AddressID }, address);
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(int id, Address address)
        {
            try
            {
                if (id != address.AddressID)
                {
                    return BadRequest("Address ID mismatch.");
                }

                await _addressService.UpdateAddressAsync(address);
                return NoContent();
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {
                await _addressService.DeleteAddressAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("getalladdresses")]
        public async Task<IActionResult> GetAddresses()
        {
            try
            {
                return Ok(await _addressService.GetAllAddressesAsync());
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("range")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddressesByRange(double x, double y, double distanceInFeet)
        {
            try
            {
                return Ok(await _addressService.GetAddressesByRangeAsync(x, y, distanceInFeet));
            }
            catch (InvalidInputException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("street/{street}")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddressesByStreet(string street)
        {
            try
            {
                return Ok(await _addressService.GetAddressesByStreetAsync(street));
            }
            catch (InvalidInputException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("locationicn/{locationIcn}")]
        public async Task<ActionResult<Address>> GetAddressByLocationIcn(int locationIcn)
        {
            try
            {
                return Ok(await _addressService.GetAddressByLocationIcnAsync(locationIcn));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidInputException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("buildingstatus/{buildingStatus}")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddressesByBuildingStatus(string buildingStatus)
        {
            try
            {
                return Ok(await _addressService.GetAddressesByBuildingStatusAsync(buildingStatus));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidInputException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}