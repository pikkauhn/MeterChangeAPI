namespace MeterChangeAPI.Interfaces
{
    public interface IEndpoint
    {
        int EndpointID { get; set; }
        int? Endpoint_SN { get; set; }
        int MeterID { get; set; }
        IMeter Meter { get; set; }
        IArcGISData? ArcGISData { get; set; } 
    }
}