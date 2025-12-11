using System.ComponentModel.DataAnnotations.Schema;

namespace AirAnalysis.DAL.Entities
{
    public class RecordDataDailyView
    {
        public int PlaceId { get; set; }
        public DateTime Date { get; set; }
        public int? PhenomenId { get; set; }
        public double? SumValue { get; set; }
        public long RecordCount { get; set; }

        [NotMapped]
        public double? AvgValue => RecordCount > 0 && SumValue.HasValue
            ? SumValue.Value / RecordCount
            : null;
    }
}