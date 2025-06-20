using MeterChangeApi.Models;

namespace MeterChangeApi.Services.CsvImport
{
    /// <summary>
    /// Defines the contract for a service that imports CSV data into the database.
    /// </summary>
    public interface ICsvImportService
    {
        /// <summary>
        /// Imports data from a CSV file according to the specified import mode.
        /// </summary>
        /// <param name="csvFileStream">The stream containing the CSV data.</param>
        /// <param name="importMode">The mode determining how to handle existing data.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ImportCsvDataAsync(Stream csvFileStream, ImportMode importMode, CancellationToken cancellationToken = default);
    }
}
