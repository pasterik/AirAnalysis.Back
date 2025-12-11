using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AirAnalysis.DAL.Entities
{
    public class RecordDataDailyViewConfig : IEntityTypeConfiguration<RecordDataDailyView>
    {
        public void Configure(EntityTypeBuilder<RecordDataDailyView> builder)
        {
            builder.ToView("vw_RecordDataDaily", schema: "dbo");
            builder.HasNoKey();

            builder.Property(e => e.PlaceId).HasColumnName("PlaceId");
            builder.Property(e => e.Date).HasColumnName("Date").HasColumnType("date");
            builder.Property(e => e.PhenomenId).HasColumnName("PhenomenId");
            builder.Property(e => e.SumValue).HasColumnType("float").HasColumnName("SumValue");
            builder.Property(e => e.RecordCount).HasColumnName("RecordCount");
        }
    }
}