namespace MeterChangeAPI.Interfaces
{
    public interface IMeter
    {
        int MeterID { get; set; }
        int? Rte_No { get; set; }
        int? Read_Sequence { get; set; }
        int? Read_Order { get; set; }
        string Meter_SN { get; set; }
        string Meter_Manufacturer { get; set; }
        string Meter_Size_Desc { get; set; }
        decimal? Lat_DD { get; set; }
        decimal? Lon_DD { get; set; }
        int? AddressID { get; set; }
        IAddress? Address { get; set; }
        int? EndpointID { get; set; }
        IEndpoint? Endpoint { get; set; }
    }
}