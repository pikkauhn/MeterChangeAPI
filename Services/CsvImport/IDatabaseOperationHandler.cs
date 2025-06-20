using MeterChangeApi.Models;

namespace MeterChangeApi.Services.CsvImport
{
    /// <summary>
    /// Defines the contract for a handler that manages database operations during CSV import.
    /// </summary>
    public interface IDatabaseOperationHandler
    {
        /// <summary>
        /// Drops all existing data from the relevant tables.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DropExistingDataAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Saves the provided entities to the database.
        /// </summary>
        /// <param name="addresses">The address entities to save.</param>
        /// <param name="wmeters">The meter entities to save.</param>
        /// <param name="wendpoints">The endpoint entities to save.</param>
        /// <param name="arcGisDatas">The ArcGIS data entities to save.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SaveDataToDatabase(List<Address> addresses, List<WMeter> wmeters, List<WEndpoint> wendpoints, List<ArcGISData> arcGisDatas, CancellationToken cancellationToken);
    }
}
