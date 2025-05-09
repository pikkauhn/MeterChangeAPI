using MeterChangeApi.Models;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Middleware.ExceptionHandling;
using Microsoft.AspNetCore.Mvc;

namespace MeterChangeApi.Controllers
{
    /// <summary>
    /// API controller for managing water meters.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MetersController(IMeterService meterService) : ControllerBase
    {
        private readonly IMeterService _meterService = meterService;

        /// <summary>
        /// Gets a specific water meter by its ID.
        /// </summary>
        /// <param name="id">The ID of the water meter to retrieve.</param>
        /// <returns>An ActionResult containing the water meter if found, or a NotFound error if not.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Wmeter>> GetMeter(int id)
        {
            try
            {
                var meter = await _meterService.GetMeterByIdAsync(id);
                return Ok(meter);
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
        /// Gets all water meters.
        /// </summary>
        /// <returns>An ActionResult containing a list of all water meters.</returns>
        [HttpGet("getallmeters")]
        public async Task<ActionResult<IEnumerable<Wmeter>>> GetMeters()
        {
            try
            {
                return Ok(await _meterService.GetAllMetersAsync());
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
        /// Gets a paginated list of water meters.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (starting from 1).</param>
        /// <param name="pageSize">The number of water meters to retrieve per page.</param>
        /// <returns>An IActionResult containing the paginated list of water meters and pagination metadata.</returns>
        [HttpGet]
        public async Task<IActionResult> GetPaginatedMeters(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest("Invalid page number or page size.");
                }

                var (meters, totalCount) = await _meterService.GetPaginatedMetersAsync(pageNumber, pageSize);
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var result = new
                {
                    Meters = meters,
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
        /// Adds a new water meter.
        /// </summary>
        /// <param name="meter">The water meter data to add.</param>
        /// <returns>An ActionResult indicating the success of the creation, including the newly created water meter.</returns>
        [HttpPost]
        public async Task<ActionResult<Wmeter>> AddMeter(Wmeter meter)
        {
            try
            {
                var createdMeter = await _meterService.CreateMeterAsync(meter);
                return CreatedAtAction(nameof(GetMeter), new { id = createdMeter.meterID }, createdMeter);
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
        /// Updates an existing water meter.
        /// </summary>
        /// <param name="id">The ID of the water meter to update.</param>
        /// <param name="meter">The updated water meter data.</param>
        /// <returns>An IActionResult indicating the success of the update (NoContent).</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMeter(int id, Wmeter meter)
        {
            try
            {
                if (id != meter.meterID)
                {
                    return BadRequest("Meter ID mismatch.");
                }
                await _meterService.UpdateMeterAsync(meter);
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
        /// Deletes a water meter by its ID.
        /// </summary>
        /// <param name="id">The ID of the water meter to delete.</param>
        /// <returns>An IActionResult indicating the success of the deletion (NoContent) or a NotFound error if the water meter doesn't exist.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeter(int id)
        {
            try
            {
                await _meterService.DeleteMeterAsync(id);
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
    }
}