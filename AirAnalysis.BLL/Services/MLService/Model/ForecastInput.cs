namespace AirAnalysis.BLL.Services.MLService.Model
{
    // ------------------------
    // DATA MODELS
    // ------------------------
    public class ForecastInput
    {
        public float Lag1 { get; set; }
        public float Lag2 { get; set; }
        public float Lag3 { get; set; }
    }
}