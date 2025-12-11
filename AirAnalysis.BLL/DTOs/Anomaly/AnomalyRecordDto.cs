namespace AirAnalysis.BLL.DTOs.Anomaly
{
    public class AnomalyRecordDto
    {
        public string Date { get; set; }
        public double Value { get; set; }
        public bool IsAnomaly { get; set; }
        public double? Mean { get; set; }
        public double? Std { get; set; }
        public double? Deviation { get; set; }
        public double? ZScore { get; set; }
    }
}