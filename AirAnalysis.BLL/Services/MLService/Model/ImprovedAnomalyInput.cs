namespace AirAnalysis.BLL.Services.MLService.Model
{
    public class ImprovedAnomalyInput
    {
        public float Value { get; set; }
        public float ZScore { get; set; }
        public float LocalZScore { get; set; }
    }
}