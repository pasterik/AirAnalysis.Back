using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.DAL.Entities
{
    public class RecordDataConfig : IEntityTypeConfiguration<RecordData>
    {
        public void Configure(EntityTypeBuilder<RecordData> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            builder.Property(c => c.Value).IsRequired();

            builder.Property(c => c.DateRecord).IsRequired();

            builder.HasOne(rd => rd.Phenomen)
                   .WithMany(p => p.RecordDatas)
                   .HasForeignKey(rd => rd.PhenomenId);

            builder.HasOne(rd => rd.Place)
                   .WithMany(p => p.RecordDatas)
                   .HasForeignKey(rd => rd.PlaceId);

            builder.ToTable("RecordData");

            builder.HasIndex(e => new { e.PlaceId, e.DateRecord, e.PhenomenId })
                    .HasDatabaseName("idx_recorddata_main");

        }
    }
}
