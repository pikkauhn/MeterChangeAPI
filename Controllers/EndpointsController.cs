using MeterChangeApi.Models;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Middleware.ExceptionHandling;
using Microsoft.AspNetCore.Mvc;

namespace MeterChangeApi.Controllers
{
    /// <summary>
    /// API controller for managing water endpoints.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EndpointsController(IEndpointService endpointService) : ControllerBase
    {
        private readonly IEndpointService _endpointService = endpointService;

        /// <summary>
        /// Gets a specific water endpoint by its ID.
        /// </summary>
        /// <param name="id">The ID of the water endpoint to retrieve.</param>
        /// <returns>An ActionResult containing the water endpoint if found, or a NotFound error if not.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<WEndpoint>> GetEndpoint(int id)
        {
            try
            {
                var endpoint = await _endpointService.GetEndpointByIdAsync(id);
                return Ok(endpoint);
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
        /// Gets all water endpoints.
        /// </summary>
        /// <returns>An ActionResult containing a list of all water endpoints.</returns>
        [HttpGet("getallendpoints")]
        public async Task<ActionResult<IEnumerable<WEndpoint>>> GetAllEndpoints()
        {
            try
            {
                return Ok(await _endpointService.GetAllEndpointsAsync());
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
        /// Gets a paginated list of water endpoints.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (starting from 1).</param>
        /// <param name="pageSize">The number of water endpoints to retrieve per page.</param>
        /// <returns>An IActionResult containing the paginated list of water endpoints and pagination metadata.</returns>
        [HttpGet]
        public async Task<IActionResult> GetPaginatedEndpoints(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest("Invalid page number or page size.");
                }

                var (endpoints, totalCount) = await _endpointService.GetPaginatedEndpointsAsync(pageNumber, pageSize);
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var result = new
                {
                    Endpoints = endpoints,
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
        /// Creates a new water endpoint.
        /// </summary>
        /// <param name="endpoint">The water endpoint data to create.</param>
        /// <returns>An ActionResult indicating the success of the creation, including the newly created water endpoint.</returns>
        [HttpPost]
        public async Task<ActionResult<WEndpoint>> CreateEndpoint(WEndpoint endpoint)
        {
            try
            {
                var createdEndpoint = await _endpointService.CreateEndpointAsync(endpoint);
                return CreatedAtAction(nameof(GetEndpoint), new { id = createdEndpoint.EndpointID }, createdEndpoint);
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
        /// Updates an existing water endpoint.
        /// </summary>
        /// <param name="id">The ID of the water endpoint to update.</param>
        /// <param name="endpoint">The updated water endpoint data.</param>
        /// <returns>An IActionResult indicating the success of the update (NoContent).</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEndpoint(int id, WEndpoint endpoint)
        {
            try
            {
                if (id != endpoint.EndpointID)
                {
                    return BadRequest("Endpoint ID mismatch.");
                }
                await _endpointService.UpdateEndpointAsync(endpoint);
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
        /// Deletes a water endpoint by its ID.
        /// </summary>
        /// <param name="id">The ID of the water endpoint to delete.</param>
        /// <returns>An IActionResult indicating the success of the deletion (NoContent) or a NotFound error if the water endpoint doesn't exist.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEndpoint(int id)
        {
            try
            {
                await _endpointService.DeleteEndpointAsync(id);
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