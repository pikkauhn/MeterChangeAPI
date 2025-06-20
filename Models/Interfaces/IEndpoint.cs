namespace MeterChangeApi.Interfaces
{
    public interface IEndpoint
    {
        int EndpointID { get; set; }
        int? Endpoint_SN { get; set; }
        int meterID { get; set; }
        IMeter meter { get; set; }
        IArcGISData? ArcGISData { get; set; }
    }
}