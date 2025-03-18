using MeterChangeApi.Models;
using MeterChangeApi.Services;
using MeterChangeAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace MeterChangeAPI.Controllers
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
            var arcGISData = await _arcGISDataService.GetArcGISDataByIdAsync(id);
            if (arcGISData == null)
            {
                return NotFound();
            }
            return arcGISData;
        }

        [HttpGet("getallarcgisdata")]
        public async Task<ActionResult<IEnumerable<ArcGISData>>> GetArcGISData()
        {
            return Ok(await _arcGISDataService.GetAllArcGISDataAsync());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArcGISData>>> GetPaginatedArcGISData(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Invalid page number or page size.");
            }

            var (endpoints, totalCount) = await _arcGISDataService.GetPaginatedArcGISDataAsync(pageNumber, pageSize);
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
        public async Task<ActionResult<ArcGISData>> CreateArcGISData(ArcGISData arcGISData)
        {
            var createdArcGISData = await _arcGISDataService.CreateArcGISDataAsync(arcGISData);
            return CreatedAtAction(nameof(GetArcGISData), new { id = createdArcGISData.ArcGISDataID }, createdArcGISData);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> CreateArcGISData(int id, ArcGISData arcGISData)
        {
            if (id != arcGISData.ArcGISDataID)
            {
                return BadRequest();
            }
            await _arcGISDataService.UpdateArcGISDataAsync(arcGISData);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArcGISData(int id)
        {
            await _arcGISDataService.DeleteArcGISDataAsync(id);
            return NoContent();
        }
    }
}