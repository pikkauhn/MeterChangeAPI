using MeterChangeApi.Models;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Middleware.ExceptionHandling;
using Microsoft.AspNetCore.Mvc;

namespace MeterChangeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EndpointsController : ControllerBase
    {
        private readonly IEndpointService _endpointService;

        public EndpointsController(IEndpointService endpointService)
        {
            _endpointService = endpointService;
        }

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

        [HttpGet]
        public async Task<IActionResult> GetPaginatedEndpoints(int pageNumber, int pageSize)
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