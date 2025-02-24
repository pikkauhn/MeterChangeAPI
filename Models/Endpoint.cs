using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeterChangeApi.Models
{
    public class Endpoint
    {
        [Key]
        public int EndpointID { get; set; }

        public int? Endpoint_SN { get; set; }

        public int MeterID { get; set; }
        
        [ForeignKey("MeterID")]
        public virtual Meter Meter { get; set; } = null!;

        public virtual ArcGISData? ArcGISData { get; set; }

    }
}