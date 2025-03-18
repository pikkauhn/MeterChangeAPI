using MeterChangeApi.Models;
using MeterChangeApi.Services;
using MeterChangeApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var endpoint = await _endpointService.GetEndpointByIdAsync(id);
            if (endpoint == null)
            {
                return NotFound();
            }
            return endpoint;
        }

        [HttpGet("getallendpoints")]
        public async Task<ActionResult<WEndpoint>> GetAllEndpoints()
        {
            return Ok(await _endpointService.GetAllEndpointsAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedEndpoints(int pageNumber, int pageSize)
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

        [HttpPost]
        public async Task<ActionResult<WEndpoint>> CreateEndpoint(WEndpoint endpoint)
        {
            var createdEndpoint = await _endpointService.CreateEndpointAsync(endpoint);
            return CreatedAtAction(nameof(GetEndpoint), new { id = createdEndpoint.EndpointID }, createdEndpoint);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEndpoint(int id, WEndpoint endpoint)
        {
            if (id != endpoint.EndpointID)
            {
                return BadRequest();
            }
            await _endpointService.UpdateEndpointAsync(endpoint);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEndpoint(int id)
        {
            await _endpointService.DeleteEndpointAsync(id);
            return NoContent();
        }
    }
}