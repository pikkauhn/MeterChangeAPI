using MeterChangeApi.Models;
using MeterChangeApi.Services.Interfaces;
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
            var meter = await _meterService.GetMeterByIdAsync(id);
            if (meter == null)
            {
                return NotFound();                
            }
            return meter;
        }

        [HttpGet("getallmeters")]
        public async Task<ActionResult<IEnumerable<Wmeter>>> GetMeters()
        {
            return Ok(await _meterService.GetAllMetersAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedMeters(int pageNumber, int pageSize)
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

        [HttpPost]
        public async Task<ActionResult<Wmeter>> AddMeter(Wmeter meter)
        {
            var createdMeter = await _meterService.CreateMeterAsync(meter);
            return CreatedAtAction(nameof(GetMeter), new { id = createdMeter.meterID }, createdMeter);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMeter(int id, Wmeter meter)
        {
            if (id != meter.meterID)
            {
                return BadRequest();
            }
            await _meterService.UpdateMeterAsync(meter);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeter(int id)
        {
            await _meterService.DeleteMeterAsync(id);
            return NoContent();
        }
    }
}