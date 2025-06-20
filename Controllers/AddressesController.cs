using MeterChangeApi.Middleware.ExceptionHandling;
using MeterChangeApi.Models;
using MeterChangeApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeterChangeApi.Controllers
{
    /// <summary>
    /// API controller for managing address information.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AddressesController(IAddressService addressService) : ControllerBase
    {
        private readonly IAddressService _addressService = addressService;

        /// <summary>
        /// Gets a specific address by its ID.
        /// </summary>
        /// <param name="id">The ID of the address to retrieve.</param>
        /// <returns>An ActionResult containing the address if found, or a NotFound error if not.</returns>
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

        /// <summary>
        /// Gets a paginated list of addresses.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (starting from 1).</param>
        /// <param name="pageSize">The number of addresses to retrieve per page.</param>
        /// <returns>An IActionResult containing the paginated list of addresses and pagination metadata.</returns>
        [HttpGet]
        public async Task<IActionResult> GetPaginatedAddresses(int pageNumber = 1, int pageSize = 10)
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

        /// <summary>
        /// Exports all addresses to a JSON file.
        /// </summary>
        /// <returns>An IActionResult containing the JSON file download.</returns>
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

        /// <summary>
        /// Creates a new address.
        /// </summary>
        /// <param name="address">The address data to create.</param>
        /// <returns>An ActionResult indicating the success of the creation, including the newly created address.</returns>
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

        /// <summary>
        /// Updates an existing address.
        /// </summary>
        /// <param name="id">The ID of the address to update.</param>
        /// <param name="address">The updated address data.</param>
        /// <returns>An IActionResult indicating the success of the update (NoContent).</returns>
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

        /// <summary>
        /// Deletes an address by its ID.
        /// </summary>
        /// <param name="id">The ID of the address to delete.</param>
        /// <returns>An IActionResult indicating the success of the deletion (NoContent) or a NotFound error if the address doesn't exist.</returns>
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

        /// <summary>
        /// Gets all addresses.
        /// </summary>
        /// <returns>An IActionResult containing a list of all addresses.</returns>
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

        /// <summary>
        /// Gets addresses within a specified geographical range.
        /// </summary>
        /// <param name="x">The X-coordinate of the center of the range.</param>
        /// <param name="y">The Y-coordinate of the center of the range.</param>
        /// <param name="distanceInFeet">The radius of the range in feet.</param>
        /// <returns>An ActionResult containing a collection of addresses within the range.</returns>
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

        /// <summary>
        /// Gets addresses located on a specific street.
        /// </summary>
        /// <param name="street">The name of the street to search for.</param>
        /// <returns>An ActionResult containing a collection of addresses on the specified street.</returns>
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

        /// <summary>
        /// Gets an address by its Location Identification Code (ICN).
        /// </summary>
        /// <param name="locationIcn">The Location ICN to search for.</param>
        /// <returns>An ActionResult containing the address with the matching ICN, or a NotFound error if not found.</returns>
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

        /// <summary>
        /// Gets addresses with a specific building status.
        /// </summary>
        /// <param name="buildingStatus">The building status to filter by.</param>
        /// <returns>An ActionResult containing a collection of addresses with the specified building status.</returns>
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