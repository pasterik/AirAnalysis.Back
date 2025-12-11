using AirAnalysis.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IBaseRepository<RecordData> RecordData { get; }
        IBaseRepository<Phenomen> Phenomen { get; }
        IBaseRepository<User> User { get; }
        IBaseRepository<RecordDataDailyView> RecordDataDailyView { get; }
        IBaseRepository<Place> Place { get; }

        Task<int> SaveAsync();
    }
}
