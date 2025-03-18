namespace MeterChangeApi.Interfaces
{
    public interface IAddress
    {
        int AddressID { get; set; }
        int? Location_ICN { get; set; }
        string Location_Address_Line1 { get; set; }
        int? Serv_Pt_ID { get; set; }
        string Mtr_Desc { get; set; }
        decimal? Location_Latitude { get; set; }
        decimal? Location_Longitude { get; set; }
        decimal? Location_Height { get; set; }
        int? Building_Year { get; set; }
        string Building_Status { get; set; }
        string City { get; set; }
        string Zip { get; set; }
        DateTime? Serv_Est_Date { get; set; }
        int? Serv_Year_Final { get; set; }
        DateTime? SL_Install_Ticket_Date { get; set; }
        string SL_Ticket_Material_US { get; set; }
        string SL_Material_Cust_Side { get; set; }
        ICollection<IMeter> meters { get; set; }
    }
}