using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AirAnalysis.DAL.Entities
{
    public class PhenomenConfig : IEntityTypeConfiguration<Phenomen>
    {
        public void Configure(EntityTypeBuilder<Phenomen> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        }
    }
}
