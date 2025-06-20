namespace MeterChangeApi.Services.Dto
{

    public class CsvDataDto
    {
        // Address related properties
        public string? Location_ICN { get; set; }
        public string? Location_Address_Line1 { get; set; }
        public string? Serv_Pt_ID { get; set; }
        public string? Mtr_Desc { get; set; }
        public string? Location_Latitude { get; set; }
        public string? Location_Longitude { get; set; }
        public string? Location_Height { get; set; }
        public string? Building_Year { get; set; }
        public string? Building_Status { get; set; }
        public string? City { get; set; }
        public string? Zip { get; set; }
        public string? Serv_Est_Date { get; set; }
        public string? Serv_Year_Final { get; set; }
        public string? SL_Install_Ticket_Date { get; set; }
        public string? SL_Material_US { get; set; }
        public string? SL_Material_Cust_Side { get; set; }

        // WMeter related properties
        public string? Rte_No { get; set; }
        public string? Read_Sequence { get; set; }
        public string? Read_Order { get; set; }
        public string? Meter_SN { get; set; }
        public string? Meter_Manufacturer { get; set; }
        public string? Meter_Size_Desc { get; set; }
        public string? Lat_DD { get; set; }
        public string? Lon_DD { get; set; }

        // WEndpoint related properties
        public string? Endpoint_SN { get; set; }

        // ArcGISData related properties
        public string? OBJECTID { get; set; }
        public string? CollectedBy { get; set; }
        public string? DeviceType { get; set; }
        public string? DeviceID { get; set; }
        public string? CorrStatus { get; set; }
        public string? CorrSource { get; set; }
        public string? CreationDateTime { get; set; }
        public string? UpdateDateTime { get; set; }
        public string? HorizEstAcc { get; set; }
        public string? VertEstAcc { get; set; }
        public string? GeomCaptureType { get; set; }
        public string? XCurrentMapCS { get; set; }
        public string? YCurrentMapCS { get; set; }
        public string? PDOP { get; set; }
        public string? HDOP { get; set; }
        public string? TaskName { get; set; }
        public string? ProjectName { get; set; }
        public string? FeatureHeight { get; set; }
        public string? AccuracyReporting { get; set; }
        public string? AutoIncrementAlpha { get; set; }
        public string? AutoIncrementNumeric { get; set; }
        public string? SL_Size { get; set; }
        public string? Class_Source_Util { get; set; }
        public string? Class_Source_Cust { get; set; }
        public string? SL_Material_All { get; set; }
    }
}
