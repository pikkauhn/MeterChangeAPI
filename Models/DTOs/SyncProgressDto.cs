namespace MeterChangeApi.Models.DTOs
{
    public class SyncProgressDto
    {
        public string Message { get; set; } = string.Empty;
        public int Percentage { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class SyncErrorDto
    {
        public string Error { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class SyncDataDto
    {
        public List<AddressDto> Addresses { get; set; } = [];
        public List<WMeterDto> Meters { get; set; } = [];
        public List<WEndpointDto> Endpoints { get; set; } = [];
        public List<ArcGISDataDto> ArcGISData { get; set; } = [];
        public DateTime SyncTimestamp { get; set; } = DateTime.UtcNow;
    }
}
