using System.ComponentModel.DataAnnotations;

namespace MeterChangeApi.Models
{
    public class Address
    {
        [Key]
        public int AddressID { get; set; }

        public int? Location_ICN { get; set; }
        public string Location_Address_Line1 { get; set; } = string.Empty;
        public int? Serv_Pt_ID { get; set; }
        public string? Mtr_Desc { get; set; } = string.Empty;
        public decimal? Location_Latitude { get; set; }
        public decimal? Location_Longitude { get; set; }
        public decimal? Location_Height { get; set; }
        public int? Building_Year { get; set; }
        public string? Building_Status { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? Zip { get; set; } = string.Empty;
        public DateTime? Serv_Est_Date { get; set; }
        public int? Serv_Year_Final { get; set; }
        public DateTime? SL_Install_Ticket_Date { get; set; }
        public string? SL_Ticket_Material_US { get; set; } = string.Empty;
        public string? SL_Material_Cust_Side { get; set; } = string.Empty;

        public virtual ICollection<WMeter> meters { get; set; } = [];
    }
}