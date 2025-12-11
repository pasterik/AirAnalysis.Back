namespace AirAnalysis.BLL.DTOs.Anomaly { 

    public record AnomalyReportDto
    {
        public int PhenomenId { get; set; }
        public int PlaceId { get; set; }
        public int TotalRecords { get; set; }
        public int AnomalyCount { get; set; }
        public double AnomalyPercentage { get; set; }
        public List<AnomalyRecordDto> Records { get; set; } = new List<AnomalyRecordDto>();
    }
}