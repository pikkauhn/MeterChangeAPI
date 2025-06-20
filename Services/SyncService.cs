using System.Text.Json;
using MeterChangeApi.Models.DTOs;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Middleware.ExceptionHandling;

namespace MeterChangeApi.Services
{
    public class SyncService : ISyncService
    {
        private readonly IAddressService _addressService;
        private readonly IMeterService _meterService;
        private readonly IEndpointService _endpointService;
        private readonly IArcGISDataService _arcGISDataService;

        public SyncService(
            IAddressService addressService,
            IMeterService meterService,
            IEndpointService endpointService,
            IArcGISDataService arcGISDataService)
        {
            _addressService = addressService;
            _meterService = meterService;
            _endpointService = endpointService;
            _arcGISDataService = arcGISDataService;
        }

        public async Task StreamSyncDataWithProgress(
            HttpResponse response,
            ILogger logger,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await WriteProgressUpdate(response, "Starting data synchronization...", 0);
                logger.LogInformation("Starting data synchronization with progress streaming");

                // Load Addresses
                await WriteProgressUpdate(response, "Loading addresses...", 10);
                logger.LogInformation("Loading addresses...");

                var addresses = await _addressService.GetAllAddressesAsync();
                var addressDtos = MapAddresses(addresses);

                await WriteProgressUpdate(response, $"Loaded {addressDtos.Count} addresses", 25);
                logger.LogInformation("Loaded {Count} addresses", addressDtos.Count);

                // Load Meters
                await WriteProgressUpdate(response, "Loading meters...", 30);
                logger.LogInformation("Loading meters...");

                var meters = await _meterService.GetAllMetersAsync();
                var meterDtos = MapMeters(meters);

                await WriteProgressUpdate(response, $"Loaded {meterDtos.Count} meters", 55);
                logger.LogInformation("Loaded {Count} meters", meterDtos.Count);

                // Load Endpoints
                await WriteProgressUpdate(response, "Loading endpoints...", 60);
                logger.LogInformation("Loading endpoints...");

                var endpoints = await _endpointService.GetAllEndpointsAsync();
                var endpointDtos = MapEndpoints(endpoints);

                await WriteProgressUpdate(response, $"Loaded {endpointDtos.Count} endpoints", 80);
                logger.LogInformation("Loaded {Count} endpoints", endpointDtos.Count);

                // Load ArcGIS Data
                await WriteProgressUpdate(response, "Loading ArcGIS data...", 85);
                logger.LogInformation("Loading ArcGIS data...");

                var arcGISData = await _arcGISDataService.GetAllArcGISDataAsync();
                var arcGISDataDtos = MapArcGISData(arcGISData);

                await WriteProgressUpdate(response, $"Loaded {arcGISDataDtos.Count} ArcGIS records", 95);
                logger.LogInformation("Loaded {Count} ArcGIS records", arcGISDataDtos.Count);

                // Finalize and send data
                await WriteProgressUpdate(response, "Finalizing data...", 98);
                logger.LogInformation("Finalizing sync data...");

                var syncData = new SyncDataDto
                {
                    Addresses = addressDtos,
                    Meters = meterDtos,
                    Endpoints = endpointDtos,
                    ArcGISData = arcGISDataDtos,
                    SyncTimestamp = DateTime.UtcNow
                };

                await WriteDataEvent(response, syncData);
                await WriteProgressUpdate(response, "Synchronization complete!", 100);

                logger.LogInformation("Data synchronization completed successfully");
            }
            catch (ServiceException ex)
            {
                logger.LogError(ex, "Service error during data synchronization: {Message}", ex.Message);
                await WriteErrorEvent(response, $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during data synchronization");
                await WriteErrorEvent(response, "An unexpected error occurred during synchronization");
            }
        }

        private static List<AddressDto> MapAddresses(IEnumerable<Models.Address> addresses)
        {
            return addresses.Select(a => new AddressDto
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
            }).ToList();
        }

        private static List<WMeterDto> MapMeters(IEnumerable<Models.WMeter> meters)
        {
            return meters.Select(m => new WMeterDto
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
            }).ToList();
        }

        private static List<WEndpointDto> MapEndpoints(IEnumerable<Models.WEndpoint> endpoints)
        {
            return endpoints.Select(e => new WEndpointDto
            {
                EndpointID = e.EndpointID,
                Endpoint_SN = e.Endpoint_SN,
                meterID = e.meterID
            }).ToList();
        }

        private static List<ArcGISDataDto> MapArcGISData(IEnumerable<Models.ArcGISData> arcGISData)
        {
            return arcGISData.Select(a => new ArcGISDataDto
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
            }).ToList();
        }

        private static async Task WriteProgressUpdate(HttpResponse response, string message, int percentage)
        {
            var progressData = new SyncProgressDto { Message = message, Percentage = percentage };
            await response.WriteAsync($"event: progress\n");
            await response.WriteAsync($"data: {JsonSerializer.Serialize(progressData)}\n\n");
            await response.Body.FlushAsync();
        }

        private static async Task WriteDataEvent(HttpResponse response, SyncDataDto data)
        {
            await response.WriteAsync($"event: data\n");
            await response.WriteAsync($"data: {JsonSerializer.Serialize(data)}\n\n");
            await response.Body.FlushAsync();
        }

        private static async Task WriteErrorEvent(HttpResponse response, string error)
        {
            var errorData = new SyncErrorDto { Error = error };
            await response.WriteAsync($"event: error\n");
            await response.WriteAsync($"data: {JsonSerializer.Serialize(errorData)}\n\n");
            await response.Body.FlushAsync();
        }
    }
}
