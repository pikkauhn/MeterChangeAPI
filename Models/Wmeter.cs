using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeterChangeApi.Models
{
    public class Wmeter
    {
        [Key]
        public int meterID { get; set; }

        public int? Rte_No { get; set; }
        public int? Read_Sequence { get; set; }
        public int? Read_Order { get; set; }
        public int meter_SN { get; set; }
        public string meter_Manufacturer { get; set; } = string.Empty;
        public string meter_Size_Desc { get; set; } = string.Empty;
        public decimal? Lat_DD { get; set; }
        public decimal? Lon_DD { get; set; }
        
        public int? AddressID { get; set; }

        [ForeignKey("AddressID")]
        public virtual Address? Address { get; set; }
        
        public virtual WEndpoint? WEndpoint { get; set; }

    }
}