using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeterChangeApi.Models
{
    public class ArcGISData
    {
        [Key]
        public int ArcGISDataID { get; set; }

        public int? OBJECTID { get; set; }

        public int EndpointID { get; set; }
        
        [ForeignKey("EndpointID")]
        public virtual WEndpoint WEndpoint { get; set; } = null!;
        public string CollectedBy { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string DeviceID { get; set; } = string.Empty;
        public string CorrStatus { get; set; } = string.Empty;
        public string CorrSource { get; set; } = string.Empty;
        public DateTime? CreationDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public decimal? HorizEstAcc { get; set; }
        public decimal? VertEstAcc { get; set; }
        public string GeomCaptureType { get; set; } = string.Empty;
        public decimal? XCurrentMapCS { get; set; }
        public decimal? YCurrentMapCS { get; set; }
        public decimal? PDOP { get; set; }
        public decimal? HDOP { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public decimal? FeatureHeight { get; set; }
        public string AccuracyReporting { get; set; } = string.Empty;
        public string AutoIncrementAlpha { get; set; } = string.Empty;
        public int? AutoIncrementNumeric { get; set; }
    }
}