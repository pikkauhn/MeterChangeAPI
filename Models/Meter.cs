using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeterChangeApi.Models
{
    public class Meter
    {
        [Key]
        public int MeterID { get; set; }

        public int? Rte_No { get; set; }
        public int? Read_Sequence { get; set; }
        public int? Read_Order { get; set; }
        public string Meter_SN { get; set; } = string.Empty;
        public string Meter_Manufacturer { get; set; } = string.Empty;
        public string Meter_Size_Desc { get; set; } = string.Empty;
        public decimal? Lat_DD { get; set; }
        public decimal? Lon_DD { get; set; }

        public int? AddressID { get; set; }

        [ForeignKey("AddressID")]
        public virtual Address? Address { get; set; }

        public int? EndpointID { get; set; }
        
        [ForeignKey("EndpointID")]
        public virtual Endpoint? Endpoint { get; set; }

    }
}