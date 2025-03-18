namespace MeterChangeApi.Interfaces
{
    public interface IMeter
    {
        int meterID { get; set; }
        int? Rte_No { get; set; }
        int? Read_Sequence { get; set; }
        int? Read_Order { get; set; }
        int meter_SN { get; set; }
        string meter_Manufacturer { get; set; }
        string meter_Size_Desc { get; set; }
        decimal? Lat_DD { get; set; }
        decimal? Lon_DD { get; set; }
        int? AddressID { get; set; }
        int? EndpointID { get; set; }
        IAddress? Address { get; set; }
        IEndpoint? WEndpoint { get; set; }
    }
}