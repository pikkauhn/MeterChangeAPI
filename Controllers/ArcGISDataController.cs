using Microsoft.AspNetCore.Mvc;
using MeterChangeApi.Models;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Middleware.ExceptionHandling;

namespace MeterChangeApi.Controllers
{
    /// <summary>
    /// API controller for managing ArcGIS data.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ArcGISDataController(IArcGISDataService arcGISDataService) : ControllerBase
    {
        private readonly IArcGISDataService _arcGISDataService = arcGISDataService;

        /// <summary>
        /// Gets a specific ArcGIS data record by its ID.
        /// </summary>
        /// <param name="id">The ID of the ArcGIS data record to retrieve.</param>
        /// <returns>An ActionResult containing the ArcGIS data if found, or a NotFound error if not.</returns>
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

        /// <summary>
        /// Gets all ArcGIS data records.
        /// </summary>
        /// <returns>An ActionResult containing a list of all ArcGIS data records.</returns>
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

        /// <summary>
        /// Gets a paginated list of ArcGIS data records.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (starting from 1).</param>
        /// <param name="pageSize">The number of ArcGIS data records to retrieve per page.</param>
        /// <returns>An ActionResult containing the paginated list of ArcGIS data and pagination metadata.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArcGISData>>> GetPaginatedArcGISData(int pageNumber = 1, int pageSize = 10)
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

        /// <summary>
        /// Creates a new ArcGIS data record.
        /// </summary>
        /// <param name="arcGISData">The ArcGIS data to create.</param>
        /// <returns>An ActionResult indicating the success of the creation, including the newly created ArcGIS data.</returns>
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

        /// <summary>
        /// Updates an existing ArcGIS data record.
        /// </summary>
        /// <param name="id">The ID of the ArcGIS data record to update.</param>
        /// <param name="arcGISData">The updated ArcGIS data.</param>
        /// <returns>An IActionResult indicating the success of the update (NoContent).</returns>
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

        /// <summary>
        /// Deletes an ArcGIS data record by its ID.
        /// </summary>
        /// <param name="id">The ID of the ArcGIS data record to delete.</param>
        /// <returns>An IActionResult indicating the success of the deletion (NoContent) or a NotFound error if the record doesn't exist.</returns>
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