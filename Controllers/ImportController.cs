using Microsoft.AspNetCore.Mvc;
using MeterChangeApi.Models;
using MeterChangeApi.Services.CsvImport;
using System.Net;

namespace MeterChangeApi.Controllers
{
    /// <summary>
    /// Controller for handling data import operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly ICsvImportService _csvImportService;
        private readonly ILogger<ImportController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportController"/> class.
        /// </summary>
        public ImportController(ICsvImportService csvImportService, ILogger<ImportController> logger)
        {
            _csvImportService = csvImportService;
            _logger = logger;
        }

        /// <summary>
        /// Imports data from a CSV file.
        /// </summary>
        /// <param name="file">The CSV file to import.</param>
        /// <param name="importMode">The mode determining how to handle existing data (default: UpdateAndAdd).</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A response indicating the result of the import operation.</returns>
        [HttpPost("csv")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [RequestSizeLimit(100_000_000)] // 100MB limit
        public async Task<IActionResult> ImportCsv(
            IFormFile file,
            [FromQuery] ImportMode importMode = ImportMode.UpdateAndAdd,
            CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.InvariantCultureIgnoreCase))
            {
                return BadRequest("Only CSV files are supported.");
            }

            try
            {
                _logger.LogInformation("Starting CSV import with mode: {ImportMode}", importMode);

                using var stream = file.OpenReadStream();
                await _csvImportService.ImportCsvDataAsync(stream, importMode, cancellationToken);

                return Ok(new { Message = "CSV import completed successfully." });
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("CSV import operation was canceled.");
                return StatusCode((int)HttpStatusCode.ServiceUnavailable, new { Message = "The import operation was canceled." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during CSV import");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred during import. See server logs for details." });
            }
        }
    }
}
