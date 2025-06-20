using MeterChangeApi.Middleware.ExceptionHandling;
using MeterChangeApi.Models;
using MeterChangeApi.Models.DTOs;
using MeterChangeApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeterChangeApi.Controllers
{
    /// <summary>
    /// API controller for managing data synchronization.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly IMeterService _meterService;
        private readonly IEndpointService _endpointService;
        private readonly IArcGISDataService _arcGISDataService;
        private readonly ISyncService _syncService;
        private readonly ILogger<SyncController> _logger;

        public SyncController(
            ILogger<SyncController> logger,
            IAddressService addressService,
            IMeterService meterService,
            IEndpointService endpointService,
            IArcGISDataService arcGISDataService,
            ISyncService syncService)
        {
            _addressService = addressService;
            _meterService = meterService;
            _endpointService = endpointService;
            _arcGISDataService = arcGISDataService;
            _syncService = syncService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all data needed for synchronization with real-time progress updates.
        /// </summary>
        /// <returns>Server-sent events stream with progress updates and final data.</returns>
        [HttpGet("getalldata-progress")]
        public async Task GetAllDataWithProgress()
        {
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");
            Response.Headers.Append("Access-Control-Allow-Origin", "*");

            await _syncService.StreamSyncDataWithProgress(Response, _logger, HttpContext.RequestAborted);
        }

        [HttpGet("getalldata-test/{limit:int?}")]
        public async Task<IActionResult> GetAllDataTest(int limit = 50)
        {
            try
            {
                _logger.LogInformation("Getting limited data for synchronization testing ({Limit} rows each).", limit);

                _logger.LogInformation("Step 1/4: Loading addresses (limited to {Limit})...", limit);
                var addresses = (await _addressService.GetPaginatedAddressesAsync(1, limit)).Item1;
                _logger.LogInformation("Loaded {Count} addresses", addresses.Count);

                _logger.LogInformation("Step 2/4: Loading meters (limited to {Limit})...", limit);
                var meters = (await _meterService.GetPaginatedMetersAsync(1, limit)).Item1;
                _logger.LogInformation("Loaded {Count} meters", meters.Count);

                _logger.LogInformation("Step 3/4: Loading endpoints (limited to {Limit})...", limit);
                var allEndpoints = await _endpointService.GetAllEndpointsAsync();
                var endpoints = allEndpoints.Take(limit);
                _logger.LogInformation("Loaded {Count} endpoints", endpoints.Count());

                _logger.LogInformation("Step 4/4: Loading ArcGIS data (limited to {Limit})...", limit);
                var allArcGISData = await _arcGISDataService.GetAllArcGISDataAsync();
                var arcGISData = allArcGISData.Take(limit);
                _logger.LogInformation("Loaded {Count} ArcGIS records", arcGISData.Count());

                _logger.LogInformation("Converting to DTOs...");
                var syncData = new SyncDataDto
                {
                    Addresses = addresses.Select(a => new AddressDto
                    {
                        AddressID = a.AddressID,
                        Location_ICN = a.Location_ICN,
                        Location_Address_Line1 = a.Location_Address_Line1,
                        Serv_Pt_ID = a.Serv_Pt_ID,
                        Mtr_Desc = a.Mtr_Desc,
                        Location_Latitude = a.Location_Latitude,
                        Location_Longitude = a.Location_Longitude,
                        Location_Height = a.Location_Height,
                        Building_Year = a.Building_Year,
                        Building_Status = a.Building_Status,
                        City = a.City,
                        Zip = a.Zip,
                        Serv_Est_Date = a.Serv_Est_Date,
                        Serv_Year_Final = a.Serv_Year_Final,
                        SL_Install_Ticket_Date = a.SL_Install_Ticket_Date,
                        SL_Ticket_Material_US = a.SL_Ticket_Material_US,
                        SL_Material_Cust_Side = a.SL_Material_Cust_Side
                    }).ToList(),

                    Meters = meters.Select(m => new WMeterDto
                    {
                        meterID = m.meterID,
                        Rte_No = m.Rte_No,
                        Read_Sequence = m.Read_Sequence,
                        Read_Order = m.Read_Order,
                        meter_SN = m.meter_SN,
                        meter_Manufacturer = m.meter_Manufacturer,
                        meter_Size_Desc = m.meter_Size_Desc,
                        Lat_DD = m.Lat_DD,
                        Lon_DD = m.Lon_DD,
                        AddressID = m.AddressID
                    }).ToList(),

                    Endpoints = endpoints.Select(e => new WEndpointDto
                    {
                        EndpointID = e.EndpointID,
                        Endpoint_SN = e.Endpoint_SN,
                        meterID = e.meterID
                    }).ToList(),

                    ArcGISData = arcGISData.Select(a => new ArcGISDataDto
                    {
                        ArcGISDataID = a.ArcGISDataID,
                        OBJECTID = a.OBJECTID,
                        EndpointID = a.EndpointID,
                        CollectedBy = a.CollectedBy,
                        DeviceType = a.DeviceType,
                        DeviceID = a.DeviceID,
                        CorrStatus = a.CorrStatus,
                        CorrSource = a.CorrSource,
                        CreationDateTime = a.CreationDateTime,
                        UpdateDateTime = a.UpdateDateTime,
                        HorizEstAcc = a.HorizEstAcc,
                        VertEstAcc = a.VertEstAcc,
                        GeomCaptureType = a.GeomCaptureType,
                        XCurrentMapCS = a.XCurrentMapCS,
                        YCurrentMapCS = a.YCurrentMapCS,
                        PDOP = a.PDOP,
                        HDOP = a.HDOP,
                        TaskName = a.TaskName,
                        ProjectName = a.ProjectName,
                        FeatureHeight = a.FeatureHeight,
                        AccuracyReporting = a.AccuracyReporting,
                        AutoIncrementAlpha = a.AutoIncrementAlpha,
                        AutoIncrementNumeric = a.AutoIncrementNumeric,
                        SL_Size = a.SL_Size,
                        Class_Source_Util = a.Class_Source_Util,
                        Class_Source_Cust = a.Class_Source_Cust,
                        SL_Material_All = a.SL_Material_All
                    }).ToList(),

                    SyncTimestamp = DateTime.UtcNow
                };

                _logger.LogInformation("Synchronization complete. Returning data.");
                return Ok(syncData);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "An error occurred while getting test data for synchronization.");
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while getting test data for synchronization.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("getalldata-test")]
        public async Task<IActionResult> GetAllDataTest()
        { 
            try
            {
                _logger.LogInformation("Getting limited data for synchronization testing (50 rows each).");

                _logger.LogInformation("Step 1/4: Loading addresses (limited to 50)...");
                var addresses = (await _addressService.GetPaginatedAddressesAsync(1, 50)).Item1;
                _logger.LogInformation("Loaded {Count} addresses", addresses.Count);

                _logger.LogInformation("Step 2/4: Loading meters (limited to 50)...");
                var meters = (await _meterService.GetPaginatedMetersAsync(1, 50)).Item1;
                _logger.LogInformation("Loaded {Count} meters", meters.Count);

                _logger.LogInformation("Step 3/4: Loading endpoints (limited to 50)...");
                var allEndpoints = await _endpointService.GetAllEndpointsAsync();
                var endpoints = allEndpoints.Take(50);
                _logger.LogInformation("Loaded {Count} endpoints", endpoints.Count());

                _logger.LogInformation("Step 4/4: Loading ArcGIS data (limited to 50)...");
                var allArcGISData = await _arcGISDataService.GetAllArcGISDataAsync();
                var arcGISData = allArcGISData.Take(50);
                _logger.LogInformation("Loaded {Count} ArcGIS records", arcGISData.Count());

                _logger.LogInformation("Converting to DTOs...");
                var syncData = new SyncDataDto
                {
                    Addresses = addresses.Select(a => new AddressDto
                    {
                        AddressID = a.AddressID,
                        Location_ICN = a.Location_ICN,
                        Location_Address_Line1 = a.Location_Address_Line1,
                        Serv_Pt_ID = a.Serv_Pt_ID,
                        Mtr_Desc = a.Mtr_Desc,
                        Location_Latitude = a.Location_Latitude,
                        Location_Longitude = a.Location_Longitude,
                        Location_Height = a.Location_Height,
                        Building_Year = a.Building_Year,
                        Building_Status = a.Building_Status,
                        City = a.City,
                        Zip = a.Zip,
                        Serv_Est_Date = a.Serv_Est_Date,
                        Serv_Year_Final = a.Serv_Year_Final,
                        SL_Install_Ticket_Date = a.SL_Install_Ticket_Date,
                        SL_Ticket_Material_US = a.SL_Ticket_Material_US,
                        SL_Material_Cust_Side = a.SL_Material_Cust_Side
                    }).ToList(),

                    Meters = meters.Select(m => new WMeterDto
                    {
                        meterID = m.meterID,
                        Rte_No = m.Rte_No,
                        Read_Sequence = m.Read_Sequence,
                        Read_Order = m.Read_Order,
                        meter_SN = m.meter_SN,
                        meter_Manufacturer = m.meter_Manufacturer,
                        meter_Size_Desc = m.meter_Size_Desc,
                        Lat_DD = m.Lat_DD,
                        Lon_DD = m.Lon_DD,
                        AddressID = m.AddressID
                    }).ToList(),

                    Endpoints = endpoints.Select(e => new WEndpointDto
                    {
                        EndpointID = e.EndpointID,
                        Endpoint_SN = e.Endpoint_SN,
                        meterID = e.meterID
                    }).ToList(),

                    ArcGISData = arcGISData.Select(a => new ArcGISDataDto
                    {
                        ArcGISDataID = a.ArcGISDataID,
                        OBJECTID = a.OBJECTID,
                        EndpointID = a.EndpointID,
                        CollectedBy = a.CollectedBy,
                        DeviceType = a.DeviceType,
                        DeviceID = a.DeviceID,
                        CorrStatus = a.CorrStatus,
                        CorrSource = a.CorrSource,
                        CreationDateTime = a.CreationDateTime,
                        UpdateDateTime = a.UpdateDateTime,
                        HorizEstAcc = a.HorizEstAcc,
                        VertEstAcc = a.VertEstAcc,
                        GeomCaptureType = a.GeomCaptureType,
                        XCurrentMapCS = a.XCurrentMapCS,
                        YCurrentMapCS = a.YCurrentMapCS,
                        PDOP = a.PDOP,
                        HDOP = a.HDOP,
                        TaskName = a.TaskName,
                        ProjectName = a.ProjectName,
                        FeatureHeight = a.FeatureHeight,
                        AccuracyReporting = a.AccuracyReporting,
                        AutoIncrementAlpha = a.AutoIncrementAlpha,
                        AutoIncrementNumeric = a.AutoIncrementNumeric,
                        SL_Size = a.SL_Size,
                        Class_Source_Util = a.Class_Source_Util,
                        Class_Source_Cust = a.Class_Source_Cust,
                        SL_Material_All = a.SL_Material_All
                    }).ToList(),

                    SyncTimestamp = DateTime.UtcNow
                };

                _logger.LogInformation("Test synchronization complete. Returning limited data.");
                return Ok(syncData);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "An error occurred while getting test data for synchronization.");
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while getting test data for synchronization.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost("updatedata")]
        public async Task<IActionResult> UpdateData([FromBody] SyncUpdateRequest updateData)
        {
            try
            {
                _logger.LogInformation("Updating data.");
                foreach (var address in updateData.Addresses ?? [])
                {
                    await _addressService.UpdateAddressAsync(address);
                }

                foreach (var meter in updateData.Meters ?? [])
                {
                    await _meterService.UpdateMeterAsync(meter);
                }

                foreach (var endpoint in updateData.Endpoints ?? [])
                {
                    await _endpointService.UpdateEndpointAsync(endpoint);
                }

                foreach (var arcGisData in updateData.ArcGISData ?? [])
                {
                    await _arcGISDataService.UpdateArcGISDataAsync(arcGisData);
                }

                return Ok(new { Message = "Data updated successfully", Timestamp = DateTime.UtcNow });
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "An error occurred while updating data.");
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating data.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
