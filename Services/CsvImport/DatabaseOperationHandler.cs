using MeterChangeApi.Data;
using MeterChangeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterChangeApi.Services.CsvImport
{
    /// <summary>
    /// Handles database operations during CSV import.
    /// </summary>
    public class DatabaseOperationHandler : IDatabaseOperationHandler
    {
        private readonly ChangeOutContext _dbContext;
        private readonly ILogger<DatabaseOperationHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseOperationHandler"/> class.
        /// </summary>
        public DatabaseOperationHandler(ChangeOutContext dbContext, ILogger<DatabaseOperationHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task DropExistingDataAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Dropping all existing data as requested by DropAndReplace import mode");

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Disable foreign key checks to allow truncating tables with relationships
                await _dbContext.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0", cancellationToken);

                // Truncate tables in reverse order of dependencies
                await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE ArcGISData", cancellationToken);
                await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Endpoints", cancellationToken);
                await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Meters", cancellationToken);
                await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Addresses", cancellationToken);

                // Re-enable foreign key checks
                await _dbContext.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1", cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                _logger.LogInformation("Successfully dropped all existing data");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error dropping existing data");
                throw new Exception("Failed to drop existing data. See inner exception for details.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task SaveDataToDatabase(List<Address> addresses, List<WMeter> wmeters, List<WEndpoint> wendpoints, List<ArcGISData> arcGisDatas, CancellationToken cancellationToken)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Disable change tracking for bulk operations
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

                // Add all entities to the context
                if (addresses.Count > 0)
                {
                    _dbContext.Addresses.AddRange(addresses);
                }

                if (wmeters.Count > 0)
                {
                    _dbContext.Meters.AddRange(wmeters);
                }

                if (wendpoints.Count > 0)
                {
                    _dbContext.Endpoints.AddRange(wendpoints);
                }

                if (arcGisDatas.Count > 0)
                {
                    _dbContext.ArcGISData.AddRange(arcGisDatas);
                }

                // Detect changes manually once for all entities
                _dbContext.ChangeTracker.DetectChanges();

                // Save all changes at once
                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                _logger.LogInformation("Transaction committed successfully with {AddressCount} addresses, {WMeterCount} wmeters, {WEndpointCount} wendpoints, and {ArcGISCount} arcGisDatas saved.",
                    addresses.Count, wmeters.Count, wendpoints.Count, arcGisDatas.Count);
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync(cancellationToken);

                // Check if it's a duplicate key exception
                if (IsDuplicateKeyException(dbEx))
                {
                    _logger.LogWarning(dbEx, "Duplicate key detected. Attempting to process records individually.");

                    // Process each entity individually to skip the problematic ones
                    await ProcessEntitiesIndividually(addresses, wmeters, wendpoints, arcGisDatas, cancellationToken);
                }
                else
                {
                    _logger.LogError(dbEx, "Database update error saving data to the database. Transaction rolled back.");
                    throw;
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error saving data to the database. Transaction rolled back.");
                throw new Exception("Error saving data to the database. See inner exception for details.", ex);
            }
            finally
            {
                // Re-enable change tracking
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        private bool IsDuplicateKeyException(DbUpdateException ex)
        {
            // Check for MySQL duplicate key exception
            if (ex.InnerException is MySqlConnector.MySqlException mySqlEx)
            {
                // MySQL error code 1062 is for "Duplicate entry"
                return mySqlEx.Number == 1062;
            }

            // For other database providers, check the error message
            return ex.InnerException?.Message?.Contains("Duplicate entry") == true ||
                   ex.InnerException?.Message?.Contains("duplicate key") == true ||
                   ex.InnerException?.Message?.Contains("UNIQUE constraint failed") == true;
        }

        private async Task ProcessEntitiesIndividually(List<Address> addresses, List<WMeter> wmeters, List<WEndpoint> wendpoints, List<ArcGISData> arcGisDatas, CancellationToken cancellationToken)
        {
            int successCount = 0;
            int failureCount = 0;

            // Clear the existing context's change tracker to avoid conflicts
            _dbContext.ChangeTracker.Clear();

            // Process addresses
            foreach (var address in addresses)
            {
                try
                {
                    // Check if address already exists
                    var existingAddress = await _dbContext.Addresses
                        .AsNoTracking() // Use AsNoTracking to avoid tracking conflicts
                        .FirstOrDefaultAsync(a =>
                            a.Location_Address_Line1 == address.Location_Address_Line1 &&
                            a.City == address.City &&
                            a.Zip == address.Zip,
                            cancellationToken);

                    if (existingAddress == null)
                    {
                        _dbContext.Addresses.Add(address);
                        await _dbContext.SaveChangesAsync(cancellationToken);
                        successCount++;
                    }
                    else
                    {
                        // Update the ID to match the existing record for relationships
                        address.AddressID = existingAddress.AddressID;
                        _logger.LogDebug("Address already exists: {Address}", address.Location_Address_Line1);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to save address {Address}", address.Location_Address_Line1);
                    failureCount++;
                }

                // Clear tracking for this entity
                _dbContext.Entry(address).State = EntityState.Detached;
            }

            // Process meters (similar pattern)
            foreach (var meter in wmeters)
            {
                try
                {
                    // Check if meter already exists
                    var existingMeter = await _dbContext.Meters
                        .AsNoTracking()
                        .FirstOrDefaultAsync(m =>
                            m.meter_SN == meter.meter_SN &&
                            m.AddressID == meter.AddressID,
                            cancellationToken);

                    if (existingMeter == null)
                    {
                        _dbContext.Meters.Add(meter);
                        await _dbContext.SaveChangesAsync(cancellationToken);
                        successCount++;
                    }
                    else
                    {
                        // Update the ID to match the existing record for relationships
                        meter.meterID = existingMeter.meterID;
                        _logger.LogDebug("Meter already exists: {MeterSN}", meter.meter_SN);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to save meter {MeterSN}", meter.meter_SN);
                    failureCount++;
                }

                // Clear tracking for this entity
                _dbContext.Entry(meter).State = EntityState.Detached;
            }

            // Process endpoints (similar pattern)
            foreach (var endpoint in wendpoints)
            {
                try
                {
                    // Check if endpoint already exists
                    var existingEndpoint = await _dbContext.Endpoints
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e =>
                            e.Endpoint_SN == endpoint.Endpoint_SN &&
                            e.meterID == endpoint.meterID,
                            cancellationToken);

                    if (existingEndpoint == null)
                    {
                        _dbContext.Endpoints.Add(endpoint);
                        await _dbContext.SaveChangesAsync(cancellationToken);
                        successCount++;
                    }
                    else
                    {
                        // Update the ID to match the existing record for relationships
                        endpoint.EndpointID = existingEndpoint.EndpointID;
                        _logger.LogDebug("Endpoint already exists: {EndpointSN}", endpoint.Endpoint_SN);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to save endpoint {EndpointSN}", endpoint.Endpoint_SN);
                    failureCount++;
                }

                // Clear tracking for this entity
                _dbContext.Entry(endpoint).State = EntityState.Detached;
            }

            // Process ArcGIS data (similar pattern)
            foreach (var arcGisData in arcGisDatas)
            {
                try
                {
                    // Check if ArcGIS data already exists by OBJECTID
                    ArcGISData? existingArcGisData = null;

                    if (arcGisData.OBJECTID.HasValue)
                    {
                        existingArcGisData = await _dbContext.ArcGISData
                            .AsNoTracking()
                            .FirstOrDefaultAsync(a => a.OBJECTID == arcGisData.OBJECTID, cancellationToken);
                    }

                    // If not found by OBJECTID, check by EndpointID
                    if (existingArcGisData == null)
                    {
                        existingArcGisData = await _dbContext.ArcGISData
                            .AsNoTracking()
                            .FirstOrDefaultAsync(a => a.EndpointID == arcGisData.EndpointID, cancellationToken);
                    }

                    if (existingArcGisData == null)
                    {
                        // If OBJECTID is causing a conflict, clear it
                        if (arcGisData.OBJECTID.HasValue)
                        {
                            var conflictingObjectId = await _dbContext.ArcGISData
                                .AsNoTracking()
                                .AnyAsync(a => a.OBJECTID == arcGisData.OBJECTID, cancellationToken);

                            if (conflictingObjectId)
                            {
                                _logger.LogWarning("OBJECTID {ObjectId} already exists but for a different endpoint. Clearing OBJECTID to avoid conflict.",
                                    arcGisData.OBJECTID);
                                arcGisData.OBJECTID = null;
                            }
                        }

                        _dbContext.ArcGISData.Add(arcGisData);
                        await _dbContext.SaveChangesAsync(cancellationToken);
                        successCount++;
                    }
                    else
                    {
                        // Update existing record (if needed)
                        _logger.LogDebug("ArcGISData already exists for endpoint {EndpointID}", arcGisData.EndpointID);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to save ArcGISData for endpoint {EndpointID}", arcGisData.EndpointID);
                    failureCount++;
                }

                // Clear tracking for this entity
                _dbContext.Entry(arcGisData).State = EntityState.Detached;
            }

            _logger.LogInformation("Individual processing completed. Success: {SuccessCount}, Failures: {FailureCount}",
                successCount, failureCount);
        }
    }
}
