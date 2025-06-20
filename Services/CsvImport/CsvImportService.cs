// File: Services\CsvImport\CsvImportService.cs

using CsvHelper;
using MeterChangeApi.Data;
using MeterChangeApi.Models;
using MeterChangeApi.Services.Dto;
using MeterChangeApi.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MeterChangeApi.Services.CsvImport
{
    public class CsvImportService : ICsvImportService
    {
        private readonly ChangeOutContext _dbContext;
        private readonly ILogger<CsvImportService> _logger;
        private const int BatchSize = 500;

        public CsvImportService(ChangeOutContext dbContext, ILogger<CsvImportService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task ImportCsvDataAsync(Stream csvFileStream, ImportMode importMode, CancellationToken cancellationToken = default)
        {
            if (importMode == ImportMode.DropAndReplace)
            {
                await DropExistingDataAsync(cancellationToken);
            }

            await ProcessCsvDataAsync(csvFileStream, importMode == ImportMode.UpdateAndAdd, cancellationToken);
        }

        private async Task DropExistingDataAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Dropping all existing data as requested by DropAndReplace import mode");

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _dbContext.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0", cancellationToken);
                await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE ArcGISData", cancellationToken);
                await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Endpoints", cancellationToken);
                await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Meters", cancellationToken);
                await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Addresses", cancellationToken);
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

        private async Task ProcessCsvDataAsync(Stream csvFileStream, bool updateExisting, CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(csvFileStream);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            var addresses = new List<Address>();
            var wmeters = new List<WMeter>();
            var wendpoints = new List<WEndpoint>();
            var arcGisDatas = new List<ArcGISData>();

            int recordCount = 0;
            int processedCount = 0;
            int skippedCount = 0;
            int updatedCount = 0;
            int addedCount = 0;

            try
            {
                await foreach (var csvRecord in csvReader.GetRecordsAsync<CsvDataDto>().WithCancellation(cancellationToken))
                {
                    recordCount++;

                    var validationResults = ValidateCsvRecord(csvRecord);
                    if (validationResults.Any())
                    {
                        _logger.LogWarning("CSV record validation failed: {Errors}", string.Join("; ", validationResults));
                        skippedCount++;
                        continue;
                    }

                    var (address, isNewAddress) = await ProcessAddressAsync(csvRecord, addresses, updateExisting);
                    if (address == null)
                    {
                        _logger.LogWarning("Skipping record due to invalid address data");
                        skippedCount++;
                        continue;
                    }

                    if (isNewAddress) addedCount++;
                    else if (updateExisting) updatedCount++;

                    var (wmeter, isNewMeter) = await ProcessWMeterAsync(csvRecord, address, wmeters, updateExisting);
                    if (wmeter == null)
                    {
                        _logger.LogWarning("Skipping record due to invalid WMeter data");
                        skippedCount++;
                        continue;
                    }

                    if (isNewMeter) addedCount++;
                    else if (updateExisting) updatedCount++;

                    var (wendpoint, isNewEndpoint) = await ProcessWEndpointAsync(csvRecord, wmeter, wendpoints, updateExisting);
                    if (wendpoint == null)
                    {
                        _logger.LogWarning("Skipping record due to invalid WEndpoint data");
                        skippedCount++;
                        continue;
                    }

                    if (isNewEndpoint) addedCount++;
                    else if (updateExisting) updatedCount++;

                    var (arcGISData, isNewArcGIS) = await ProcessArcGISDataAsync(csvRecord, wendpoint, updateExisting);
                    if (arcGISData != null)
                    {
                        arcGisDatas.Add(arcGISData);
                        if (isNewArcGIS) addedCount++;
                        else if (updateExisting) updatedCount++;
                    }

                    processedCount++;

                    if (processedCount % BatchSize == 0)
                    {
                        await SaveDataToDatabase(addresses, wmeters, wendpoints, arcGisDatas, cancellationToken);
                        addresses.Clear();
                        wmeters.Clear();
                        wendpoints.Clear();
                        arcGisDatas.Clear();

                        _logger.LogInformation("Processed {ProcessedCount} of {RecordCount} records",
                            processedCount, recordCount);
                    }
                }

                if (addresses.Count > 0 || wmeters.Count > 0 || wendpoints.Count > 0 || arcGisDatas.Count > 0)
                {
                    await SaveDataToDatabase(addresses, wmeters, wendpoints, arcGisDatas, cancellationToken);
                }

                _logger.LogInformation("Import completed. Total records: {RecordCount}, Processed: {ProcessedCount}, " +
                                      "Added: {AddedCount}, Updated: {UpdatedCount}, Skipped: {SkippedCount}",
                    recordCount, processedCount, addedCount, updatedCount, skippedCount);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("CSV import operation was canceled.");
                throw;
            }
            catch (FormatException fex)
            {
                _logger.LogError(fex, "CSV format error encountered.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading CSV records");
                throw;
            }
        }

        private static IEnumerable<string> ValidateCsvRecord(CsvDataDto record)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(record, serviceProvider: null, items: null);
            Validator.TryValidateObject(record, context, validationResults, validateAllProperties: true);
            return validationResults.Select(vr => vr.ErrorMessage ?? "Unknown validation error");
        }

        private async Task<(Address?, bool)> ProcessAddressAsync(CsvDataDto csvRecord, List<Address> addresses, bool updateExisting)
        {
            var missingFields = CsvImportHelper.ValidateRequiredStringFields(csvRecord,
            [
                nameof(CsvDataDto.Location_Address_Line1),
            ]);

            if (missingFields.Count > 0)
            {
                _logger.LogWarning("Skipping record due to missing required address fields: {MissingFields}",
                    string.Join(", ", missingFields));
                return (null, false);
            }

            try
            {
                var existingAddress = addresses.FirstOrDefault(a =>
                    string.Equals(a.Location_Address_Line1, csvRecord.Location_Address_Line1, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(a.City, csvRecord.City, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(a.Zip, csvRecord.Zip, StringComparison.OrdinalIgnoreCase));

                if (existingAddress != null)
                {
                    return (existingAddress, false);
                }

                var locationIcn = CsvImportHelper.ParseIntOrNull(csvRecord.Location_ICN);
                Address? dbAddress = null;

                if (locationIcn.HasValue)
                {
                    dbAddress = await _dbContext.Addresses
                        .FirstOrDefaultAsync(a => a.Location_ICN == locationIcn);
                }

                if (dbAddress == null)
                {
                    dbAddress = await _dbContext.Addresses
                        .FirstOrDefaultAsync(a =>
                            a.Location_Address_Line1 == csvRecord.Location_Address_Line1 &&
                            a.City == csvRecord.City &&
                            a.Zip == csvRecord.Zip);
                }

                if (dbAddress != null)
                {
                    if (updateExisting)
                    {
                        dbAddress.Location_ICN = locationIcn;
                        dbAddress.Location_Address_Line1 = csvRecord.Location_Address_Line1!;
                        dbAddress.Serv_Pt_ID = CsvImportHelper.ParseIntOrNull(csvRecord.Serv_Pt_ID);
                        dbAddress.Mtr_Desc = csvRecord.Mtr_Desc!;
                        dbAddress.Location_Latitude = CsvImportHelper.ParseDecimalOrNull(csvRecord.Location_Latitude);
                        dbAddress.Location_Longitude = CsvImportHelper.ParseDecimalOrNull(csvRecord.Location_Longitude);
                        dbAddress.Location_Height = CsvImportHelper.ParseDecimalOrNull(csvRecord.Location_Height);
                        dbAddress.Building_Year = CsvImportHelper.ParseIntOrNull(csvRecord.Building_Year);
                        dbAddress.Building_Status = csvRecord.Building_Status!;
                        dbAddress.City = csvRecord.City!;
                        dbAddress.Zip = csvRecord.Zip!;
                        dbAddress.Serv_Est_Date = CsvImportHelper.ParseDateTimeOrNull(csvRecord.Serv_Est_Date);
                        dbAddress.Serv_Year_Final = CsvImportHelper.ParseIntOrNull(csvRecord.Serv_Year_Final);
                        dbAddress.SL_Install_Ticket_Date = CsvImportHelper.ParseDateTimeOrNull(csvRecord.SL_Install_Ticket_Date);
                        dbAddress.SL_Ticket_Material_US = csvRecord.SL_Material_US!;
                        dbAddress.SL_Material_Cust_Side = csvRecord.SL_Material_Cust_Side!;

                        _dbContext.Addresses.Update(dbAddress);
                    }

                    return (dbAddress, false);
                }

                var address = new Address
                {
                    Location_ICN = locationIcn,
                    Location_Address_Line1 = csvRecord.Location_Address_Line1!,
                    Serv_Pt_ID = CsvImportHelper.ParseIntOrNull(csvRecord.Serv_Pt_ID),
                    Mtr_Desc = csvRecord.Mtr_Desc!,
                    Location_Latitude = CsvImportHelper.ParseDecimalOrNull(csvRecord.Location_Latitude),
                    Location_Longitude = CsvImportHelper.ParseDecimalOrNull(csvRecord.Location_Longitude),
                    Location_Height = CsvImportHelper.ParseDecimalOrNull(csvRecord.Location_Height),
                    Building_Year = CsvImportHelper.ParseIntOrNull(csvRecord.Building_Year),
                    Building_Status = csvRecord.Building_Status!,
                    City = csvRecord.City!,
                    Zip = csvRecord.Zip!,
                    Serv_Est_Date = CsvImportHelper.ParseDateTimeOrNull(csvRecord.Serv_Est_Date),
                    Serv_Year_Final = CsvImportHelper.ParseIntOrNull(csvRecord.Serv_Year_Final),
                    SL_Install_Ticket_Date = CsvImportHelper.ParseDateTimeOrNull(csvRecord.SL_Install_Ticket_Date),
                    SL_Ticket_Material_US = csvRecord.SL_Material_US!,
                    SL_Material_Cust_Side = csvRecord.SL_Material_Cust_Side!
                };

                addresses.Add(address);
                return (address, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing address. Record: {@CsvRecord}", csvRecord);
                return (null, false);
            }
        }

        private async Task<(WMeter?, bool)> ProcessWMeterAsync(CsvDataDto csvRecord, Address address, List<WMeter> wmeters, bool updateExisting)
        {
            var missingFields = CsvImportHelper.ValidateRequiredStringFields(csvRecord, new[]
            {
                nameof(CsvDataDto.Meter_Manufacturer),
                nameof(CsvDataDto.Meter_Size_Desc),
                nameof(CsvDataDto.Meter_SN)
            });

            if (missingFields.Count > 0)
            {
                _logger.LogWarning("Skipping record due to missing required WMeter fields: {MissingFields}",
                    string.Join(", ", missingFields));
                return (null, false);
            }

            var meterSN = CsvImportHelper.ParseIntOrNull(csvRecord.Meter_SN);
            if (!meterSN.HasValue)
            {
                _logger.LogWarning("Meter_SN is required and invalid. Record: {@CsvRecord}", csvRecord);
                return (null, false);
            }

            try
            {
                var existingMeter = wmeters.FirstOrDefault(m =>
                    m.meter_SN == meterSN.Value &&
                    m.Address?.AddressID == address.AddressID);

                if (existingMeter != null)
                {
                    return (existingMeter, false);
                }

                var dbMeter = await _dbContext.Meters
                    .FirstOrDefaultAsync(m =>
                        m.meter_SN == meterSN.Value &&
                        m.AddressID == address.AddressID);

                if (dbMeter != null)
                {
                    if (updateExisting)
                    {
                        dbMeter.Rte_No = CsvImportHelper.ParseIntOrNull(csvRecord.Rte_No);
                        dbMeter.Read_Sequence = CsvImportHelper.ParseIntOrNull(csvRecord.Read_Sequence);
                        dbMeter.Read_Order = CsvImportHelper.ParseIntOrNull(csvRecord.Read_Order);
                        dbMeter.meter_Manufacturer = csvRecord.Meter_Manufacturer!;
                        dbMeter.meter_Size_Desc = csvRecord.Meter_Size_Desc!;
                        dbMeter.Lat_DD = CsvImportHelper.ParseDecimalOrNull(csvRecord.Lat_DD);
                        dbMeter.Lon_DD = CsvImportHelper.ParseDecimalOrNull(csvRecord.Lon_DD);

                        _dbContext.Meters.Update(dbMeter);
                    }

                    return (dbMeter, false);
                }

                var wmeter = new WMeter
                {
                    Rte_No = CsvImportHelper.ParseIntOrNull(csvRecord.Rte_No),
                    Read_Sequence = CsvImportHelper.ParseIntOrNull(csvRecord.Read_Sequence),
                    Read_Order = CsvImportHelper.ParseIntOrNull(csvRecord.Read_Order),
                    meter_SN = meterSN.Value,
                    meter_Manufacturer = csvRecord.Meter_Manufacturer!,
                    meter_Size_Desc = csvRecord.Meter_Size_Desc!,
                    Lat_DD = CsvImportHelper.ParseDecimalOrNull(csvRecord.Lat_DD),
                    Lon_DD = CsvImportHelper.ParseDecimalOrNull(csvRecord.Lon_DD),
                    Address = address
                };

                wmeters.Add(wmeter);
                return (wmeter, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing wmeter. Record: {@CsvRecord}", csvRecord);
                return (null, false);
            }
        }

        private async Task<(WEndpoint?, bool)> ProcessWEndpointAsync(CsvDataDto csvRecord, WMeter wmeter, List<WEndpoint> wendpoints, bool updateExisting)
        {
            try
            {
                var endpointSN = CsvImportHelper.ParseIntOrNull(csvRecord.Endpoint_SN);
                if (!endpointSN.HasValue)
                {
                    _logger.LogWarning("Endpoint_SN is required and invalid. Record: {@CsvRecord}", csvRecord);
                    return (null, false);
                }

                // Check in current batch first
                var existingEndpoint = wendpoints.FirstOrDefault(e =>
                    e.Endpoint_SN == endpointSN &&
                    e.meter?.meterID == wmeter.meterID);

                if (existingEndpoint != null)
                {
                    return (existingEndpoint, false);
                }

                // Check in database for duplication
                var dbEndpoint = await _dbContext.Endpoints
                    .FirstOrDefaultAsync(e =>
                        e.Endpoint_SN == endpointSN &&
                        e.meterID == wmeter.meterID);

                if (dbEndpoint != null)
                {
                    if (updateExisting)
                    {
                        // Update properties if needed
                        dbEndpoint.Endpoint_SN = endpointSN;
                        _dbContext.Endpoints.Update(dbEndpoint);
                    }
                    return (dbEndpoint, false);
                }

                // Create new endpoint
                var wendpoint = new WEndpoint
                {
                    Endpoint_SN = endpointSN,
                    meter = wmeter
                };

                wendpoints.Add(wendpoint);
                return (wendpoint, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing wendpoint. Record: {@CsvRecord}", csvRecord);
                return (null, false);
            }
        }

        private async Task<(ArcGISData?, bool)> ProcessArcGISDataAsync(CsvDataDto csvRecord, WEndpoint wendpoint, bool updateExisting)
        {
            try
            {
                var objectId = CsvImportHelper.ParseIntOrNull(csvRecord.OBJECTID);
                ArcGISData? dbArcGISData = null;

                if (objectId.HasValue)
                {
                    dbArcGISData = await _dbContext.ArcGISData
                        .FirstOrDefaultAsync(a => a.OBJECTID == objectId);
                }

                if (dbArcGISData == null)
                {
                    dbArcGISData = await _dbContext.ArcGISData
                        .FirstOrDefaultAsync(a => a.EndpointID == wendpoint.EndpointID);
                }

                if (dbArcGISData != null)
                {
                    if (updateExisting)
                    {
                        // Update properties
                        dbArcGISData.OBJECTID = objectId;
                        dbArcGISData.CollectedBy = csvRecord.CollectedBy;
                        dbArcGISData.DeviceType = csvRecord.DeviceType;
                        dbArcGISData.DeviceID = csvRecord.DeviceID;
                        dbArcGISData.CorrStatus = csvRecord.CorrStatus;
                        dbArcGISData.CorrSource = csvRecord.CorrSource;
                        dbArcGISData.CreationDateTime = CsvImportHelper.ParseDateTimeOrNull(csvRecord.CreationDateTime);
                        dbArcGISData.UpdateDateTime = CsvImportHelper.ParseDateTimeOrNull(csvRecord.UpdateDateTime);
                        dbArcGISData.HorizEstAcc = CsvImportHelper.ParseDecimalOrNull(csvRecord.HorizEstAcc);
                        dbArcGISData.VertEstAcc = CsvImportHelper.ParseDecimalOrNull(csvRecord.VertEstAcc);
                        dbArcGISData.GeomCaptureType = csvRecord.GeomCaptureType;
                        dbArcGISData.XCurrentMapCS = CsvImportHelper.ParseDecimalOrNull(csvRecord.XCurrentMapCS);
                        dbArcGISData.YCurrentMapCS = CsvImportHelper.ParseDecimalOrNull(csvRecord.YCurrentMapCS);
                        dbArcGISData.PDOP = CsvImportHelper.ParseDecimalOrNull(csvRecord.PDOP);
                        dbArcGISData.HDOP = CsvImportHelper.ParseDecimalOrNull(csvRecord.HDOP);
                        dbArcGISData.TaskName = csvRecord.TaskName;
                        dbArcGISData.ProjectName = csvRecord.ProjectName;
                        dbArcGISData.FeatureHeight = CsvImportHelper.ParseDecimalOrNull(csvRecord.FeatureHeight);
                        dbArcGISData.AccuracyReporting = csvRecord.AccuracyReporting;
                        dbArcGISData.AutoIncrementAlpha = csvRecord.AutoIncrementAlpha;
                        dbArcGISData.AutoIncrementNumeric = CsvImportHelper.ParseIntOrNull(csvRecord.AutoIncrementNumeric);
                        dbArcGISData.SL_Size = csvRecord.SL_Size;
                        dbArcGISData.Class_Source_Util = csvRecord.Class_Source_Util;
                        dbArcGISData.Class_Source_Cust = csvRecord.Class_Source_Cust;
                        dbArcGISData.SL_Material_All = csvRecord.SL_Material_All;

                        _dbContext.ArcGISData.Update(dbArcGISData);
                    }
                    return (dbArcGISData, false);
                }

                // Create new ArcGISData
                var arcGISData = new ArcGISData
                {
                    OBJECTID = objectId,
                    WEndpoint = wendpoint,
                    CollectedBy = csvRecord.CollectedBy,
                    DeviceType = csvRecord.DeviceType,
                    DeviceID = csvRecord.DeviceID,
                    CorrStatus = csvRecord.CorrStatus,
                    CorrSource = csvRecord.CorrSource,
                    CreationDateTime = CsvImportHelper.ParseDateTimeOrNull(csvRecord.CreationDateTime),
                    UpdateDateTime = CsvImportHelper.ParseDateTimeOrNull(csvRecord.UpdateDateTime),
                    HorizEstAcc = CsvImportHelper.ParseDecimalOrNull(csvRecord.HorizEstAcc),
                    VertEstAcc = CsvImportHelper.ParseDecimalOrNull(csvRecord.VertEstAcc),
                    GeomCaptureType = csvRecord.GeomCaptureType,
                    XCurrentMapCS = CsvImportHelper.ParseDecimalOrNull(csvRecord.XCurrentMapCS),
                    YCurrentMapCS = CsvImportHelper.ParseDecimalOrNull(csvRecord.YCurrentMapCS),
                    PDOP = CsvImportHelper.ParseDecimalOrNull(csvRecord.PDOP),
                    HDOP = CsvImportHelper.ParseDecimalOrNull(csvRecord.HDOP),
                    TaskName = csvRecord.TaskName,
                    ProjectName = csvRecord.ProjectName,
                    FeatureHeight = CsvImportHelper.ParseDecimalOrNull(csvRecord.FeatureHeight),
                    AccuracyReporting = csvRecord.AccuracyReporting,
                    AutoIncrementAlpha = csvRecord.AutoIncrementAlpha,
                    AutoIncrementNumeric = CsvImportHelper.ParseIntOrNull(csvRecord.AutoIncrementNumeric),
                    SL_Size = csvRecord.SL_Size,
                    Class_Source_Util = csvRecord.Class_Source_Util,
                    Class_Source_Cust = csvRecord.Class_Source_Cust,
                    SL_Material_All = csvRecord.SL_Material_All
                };

                return (arcGISData, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing arcgisdata. Record: {@CsvRecord}", csvRecord);
                return (null, false);
            }
        }

        private async Task SaveDataToDatabase(
    List<Address> addresses,
    List<WMeter> wmeters,
    List<WEndpoint> wendpoints,
    List<ArcGISData> arcGisDatas,
    CancellationToken cancellationToken)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Handle Addresses
                foreach (var address in addresses)
                {
                    var existingAddress = await _dbContext.Addresses
                        .FirstOrDefaultAsync(a => a.Location_ICN == address.Location_ICN, cancellationToken);
                    if (existingAddress != null)
                    {
                        // Update existing address
                        existingAddress.Location_Address_Line1 = address.Location_Address_Line1;
                        existingAddress.Serv_Pt_ID = address.Serv_Pt_ID;
                        existingAddress.Mtr_Desc = address.Mtr_Desc;
                        existingAddress.Location_Latitude = address.Location_Latitude;
                        existingAddress.Location_Longitude = address.Location_Longitude;
                        existingAddress.Location_Height = address.Location_Height;
                        existingAddress.Building_Year = address.Building_Year;
                        existingAddress.Building_Status = address.Building_Status;
                        existingAddress.City = address.City;
                        existingAddress.Zip = address.Zip;
                        existingAddress.Serv_Est_Date = address.Serv_Est_Date;
                        existingAddress.Serv_Year_Final = address.Serv_Year_Final;
                        existingAddress.SL_Install_Ticket_Date = address.SL_Install_Ticket_Date;
                        existingAddress.SL_Ticket_Material_US = address.SL_Ticket_Material_US;
                        existingAddress.SL_Material_Cust_Side = address.SL_Material_Cust_Side;
                        _dbContext.Addresses.Update(existingAddress);
                    }
                    else
                    {
                        _dbContext.Addresses.Add(address);
                    }
                }

                // Handle WMeters
                foreach (var meter in wmeters)
                {
                    int? addressId = null;
                    if (meter.Address != null)
                    {
                        addressId = meter.Address.AddressID;
                    }

                    var existingMeter = await _dbContext.Meters
                        .FirstOrDefaultAsync(m => m.meter_SN == meter.meter_SN && m.AddressID == addressId, cancellationToken);

                    if (existingMeter != null)
                    {
                        // Update existing meter
                        existingMeter.Rte_No = meter.Rte_No;
                        existingMeter.Read_Sequence = meter.Read_Sequence;
                        existingMeter.Read_Order = meter.Read_Order;
                        existingMeter.meter_Manufacturer = meter.meter_Manufacturer;
                        existingMeter.meter_Size_Desc = meter.meter_Size_Desc;
                        existingMeter.Lat_DD = meter.Lat_DD;
                        existingMeter.Lon_DD = meter.Lon_DD;
                        _dbContext.Meters.Update(existingMeter);
                    }
                    else
                    {
                        _dbContext.Meters.Add(meter);
                    }
                }

                // Handle WEndpoints
                foreach (var endpoint in wendpoints)
                {
                    var existingEndpoint = await _dbContext.Endpoints
                        .FirstOrDefaultAsync(e => e.Endpoint_SN == endpoint.Endpoint_SN && e.meter.meterID == endpoint.meter.meterID, cancellationToken);
                    if (existingEndpoint != null)
                    {
                        // Update existing endpoint if needed
                        existingEndpoint.Endpoint_SN = endpoint.Endpoint_SN;
                        _dbContext.Endpoints.Update(existingEndpoint);
                    }
                    else
                    {
                        _dbContext.Endpoints.Add(endpoint);
                    }
                }

                // Handle ArcGISData
                foreach (var arcData in arcGisDatas)
                {
                    var existingArcData = await _dbContext.ArcGISData
                        .FirstOrDefaultAsync(a => a.OBJECTID == arcData.OBJECTID, cancellationToken);
                    if (existingArcData != null)
                    {
                        // Update existing ArcGISData
                        existingArcData.CollectedBy = arcData.CollectedBy;
                        existingArcData.DeviceType = arcData.DeviceType;
                        existingArcData.DeviceID = arcData.DeviceID;
                        existingArcData.CorrStatus = arcData.CorrStatus;
                        existingArcData.CorrSource = arcData.CorrSource;
                        existingArcData.CreationDateTime = arcData.CreationDateTime;
                        existingArcData.UpdateDateTime = arcData.UpdateDateTime;
                        existingArcData.HorizEstAcc = arcData.HorizEstAcc;
                        existingArcData.VertEstAcc = arcData.VertEstAcc;
                        existingArcData.GeomCaptureType = arcData.GeomCaptureType;
                        existingArcData.XCurrentMapCS = arcData.XCurrentMapCS;
                        existingArcData.YCurrentMapCS = arcData.YCurrentMapCS;
                        existingArcData.PDOP = arcData.PDOP;
                        existingArcData.HDOP = arcData.HDOP;
                        existingArcData.TaskName = arcData.TaskName;
                        existingArcData.ProjectName = arcData.ProjectName;
                        existingArcData.FeatureHeight = arcData.FeatureHeight;
                        existingArcData.AccuracyReporting = arcData.AccuracyReporting;
                        existingArcData.AutoIncrementAlpha = arcData.AutoIncrementAlpha;
                        existingArcData.AutoIncrementNumeric = arcData.AutoIncrementNumeric;
                        existingArcData.SL_Size = arcData.SL_Size;
                        existingArcData.Class_Source_Util = arcData.Class_Source_Util;
                        existingArcData.Class_Source_Cust = arcData.Class_Source_Cust;
                        existingArcData.SL_Material_All = arcData.SL_Material_All;
                        _dbContext.ArcGISData.Update(existingArcData);
                    }
                    else
                    {
                        _dbContext.ArcGISData.Add(arcData);
                    }
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                _logger.LogInformation("Transaction committed successfully with {AddressCount} addresses, {WMeterCount} wmeters, {WEndpointCount} wendpoints, and {ArcGISCount} arcGisDatas saved.",
                    addresses.Count, wmeters.Count, wendpoints.Count, arcGisDatas.Count);
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(dbEx, "Database update error saving data to the database. Transaction rolled back.");
                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error saving data to the database. Transaction rolled back.");
                throw new Exception("Error saving data to the database. See inner exception for details.", ex);
            }
        }
    }
}