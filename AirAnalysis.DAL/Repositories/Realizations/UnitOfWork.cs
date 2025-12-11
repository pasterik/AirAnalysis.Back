using AirAnalysis.DAL.Data;
using AirAnalysis.DAL.Entities;
using AirAnalysis.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.DAL.Repositories.Realizations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AirAnalysisDbContext _context;

        public IBaseRepository<RecordData> RecordData { get; }
        public IBaseRepository<User> User { get; }

        public IBaseRepository<Phenomen> Phenomen { get; }

        public IBaseRepository<Place> Place { get; }

        public IBaseRepository<RecordDataDailyView> RecordDataDailyView { get; }

        public UnitOfWork(AirAnalysisDbContext context)
        {
            _context = context;
            RecordData = new BaseRepository<RecordData>(context);
            User = new BaseRepository<User>(context);
            Phenomen = new BaseRepository<Phenomen>(context);
            Place = new BaseRepository<Place>(context);
        }

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
    }
}
