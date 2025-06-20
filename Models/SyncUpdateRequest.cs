using MeterChangeApi.Models;

namespace MeterChangeApi.Models
{
    public class SyncUpdateRequest
    {
        public IEnumerable<Address>? Addresses { get; set; }
        public IEnumerable<WMeter>? Meters { get; set; }
        public IEnumerable<WEndpoint>? Endpoints { get; set; }
        public IEnumerable<ArcGISData>? ArcGISData { get; set; }
        public DateTime RequestTimestamp { get; set; } = DateTime.UtcNow;
    }
}
