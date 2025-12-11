namespace AirAnalysis.BLL.DTOs.CsvRecord
{
    public class SaveBot
    {
        public string? device_id { get; set; } // Поле "device_id" у CSV
        public string? phenomenon { get; set; } // Поле "phenomen" у CSV
        public double? value { get; set; }    // Поле "value" у CSV
        public DateTime? logged_at { get; set; } // Поле "logged_at" у CSV
        public string? value_text { get; set; } // Поле "value_text" у CSV

    }

}
