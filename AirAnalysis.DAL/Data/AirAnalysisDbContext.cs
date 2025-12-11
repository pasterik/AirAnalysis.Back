using AirAnalysis.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace AirAnalysis.DAL.Data
{
    public class AirAnalysisDbContext : DbContext
    {
        public AirAnalysisDbContext(DbContextOptions<AirAnalysisDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> User { get; set; }
        public DbSet<RecordData> RecordDatas { get; set; }
        public DbSet<Phenomen> Phenomens { get; set; } 
        public DbSet<Place> Places { get; set; }
        public DbSet<RecordDataDailyView> RecordDataDailyView { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AirAnalysisDbContext).Assembly);
        }
    }
}
