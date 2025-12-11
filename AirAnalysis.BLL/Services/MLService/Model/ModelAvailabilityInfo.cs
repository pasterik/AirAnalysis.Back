namespace AirAnalysis.BLL.Services.MLService.Model
{
    public class ModelAvailabilityInfo
    {
        public List<int> AvailableForecastModels { get; set; }
        public List<int> AvailableAnomalyModels { get; set; }
        public bool HasClassificationModel { get; set; }
    }
}