namespace AirAnalysis.BLL.Services.MLService.Model
{
    public class TimeseriesForecast
    {
        public DateTime Date { get; set; }
        public float PredictedValue { get; set; }
    }
}