namespace MeterChangeApi.Interfaces
{
    public interface IArcGISData
    {
        int ArcGISDataID { get; set; }
        int? OBJECTID { get; set; }
        int EndpointID { get; set; }
        IEndpoint WEndpoint { get; set; } // Or IEndpoint if you have an IEndpoint interface
        string CollectedBy { get; set; }
        string DeviceType { get; set; }
        string DeviceID { get; set; }
        string CorrStatus { get; set; }
        string CorrSource { get; set; }
        DateTime? CreationDateTime { get; set; }
        DateTime? UpdateDateTime { get; set; }
        decimal? HorizEstAcc { get; set; }
        decimal? VertEstAcc { get; set; }
        string GeomCaptureType { get; set; }
        decimal? XCurrentMapCS { get; set; }
        decimal? YCurrentMapCS { get; set; }
        decimal? PDOP { get; set; }
        decimal? HDOP { get; set; }
        string TaskName { get; set; }
        string ProjectName { get; set; }
        decimal? FeatureHeight { get; set; }
        string AccuracyReporting { get; set; }
        string AutoIncrementAlpha { get; set; }
        int? AutoIncrementNumeric { get; set; }
    }
}