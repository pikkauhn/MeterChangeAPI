using Microsoft.AspNetCore.Mvc;
using MeterChangeApi.Models;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Middleware.ExceptionHandling;

namespace MeterChangeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArcGISDataController : ControllerBase
    {
        private readonly IArcGISDataService _arcGISDataService;

        public ArcGISDataController(IArcGISDataService arcGISDataService)
        {
            _arcGISDataService = arcGISDataService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArcGISData>> GetArcGISData(int id)
        {
            try
            {
                var arcGISData = await _arcGISDataService.GetArcGISDataByIdAsync(id);
                return Ok(arcGISData);
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

        [HttpGet("getallarcgisdata")]
        public async Task<ActionResult<IEnumerable<ArcGISData>>> GetArcGISData()
        {
            try
            {
                return Ok(await _arcGISDataService.GetAllArcGISDataAsync());
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
        public async Task<ActionResult<IEnumerable<ArcGISData>>> GetPaginatedArcGISData(int pageNumber, int pageSize)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest("Invalid page number or page size.");
                }

                var (arcGISData, totalCount) = await _arcGISDataService.GetPaginatedArcGISDataAsync(pageNumber, pageSize);
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var result = new
                {
                    ArcGISData = arcGISData,
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
        public async Task<ActionResult<ArcGISData>> CreateArcGISData(ArcGISData arcGISData)
        {
            try
            {
                var createdArcGISData = await _arcGISDataService.CreateArcGISDataAsync(arcGISData);
                return CreatedAtAction(nameof(GetArcGISData), new { id = createdArcGISData.ArcGISDataID }, createdArcGISData);
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
        public async Task<IActionResult> UpdateArcGISData(int id, ArcGISData arcGISData)
        {
            try
            {
                if (id != arcGISData.ArcGISDataID)
                {
                    return BadRequest("ArcGISData ID mismatch.");
                }
                await _arcGISDataService.UpdateArcGISDataAsync(arcGISData);
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
        public async Task<IActionResult> DeleteArcGISData(int id)
        {
            try
            {
                await _arcGISDataService.DeleteArcGISDataAsync(id);
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