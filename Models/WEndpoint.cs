using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeterChangeApi.Models
{
    public class WEndpoint
    {
        public WEndpoint() { }
        [Key]
        public int EndpointID { get; set; }

        public int? Endpoint_SN { get; set; }

        public int meterID { get; set; }
        
        [ForeignKey("meterID")]
        public virtual Wmeter meter { get; set; } = null!;

        public virtual ArcGISData? ArcGISData { get; set; }

    }
}