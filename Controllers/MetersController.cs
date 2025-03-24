using MeterChangeApi.Models;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Middleware.ExceptionHandling;
using Microsoft.AspNetCore.Mvc;

namespace MeterChangeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetersController : ControllerBase
    {
        private readonly IMeterService _meterService;

        public MetersController(IMeterService meterService)
        {
            _meterService = meterService;
        }

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

        [HttpGet]
        public async Task<IActionResult> GetPaginatedMeters(int pageNumber, int pageSize)
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