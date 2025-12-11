namespace AirAnalysis.BLL.Services.MLService.Model
{
    public class AnomalyResult
    {
        public double Value { get; set; }
        public bool IsAnomaly { get; set; }
        public double? Mean { get; set; }
        public double? Std { get; set; }
        public double? DeviationFromMean { get; set; }
        public double? ZScore { get; set; }
        public string Error { get; set; }
    }
}