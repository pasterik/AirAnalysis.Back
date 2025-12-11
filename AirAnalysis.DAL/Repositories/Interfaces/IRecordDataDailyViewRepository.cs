using AirAnalysis.DAL.Entities;
using AirAnalysis.DAL.Options;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace AirAnalysis.DAL.Repositories.Interfaces
{
    public interface IRecordDataDailyViewRepository : IBaseRepository<RecordDataDailyView>
    {
    }
}
