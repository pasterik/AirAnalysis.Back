using AirAnalysis.DAL.Data;
using AirAnalysis.DAL.Entities;
using AirAnalysis.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.DAL.Repositories.Realizations
{
    public class RecordDataDailyViewRepository : BaseRepository<RecordData>, IRecordDataRepository
    {
        public RecordDataDailyViewRepository(AirAnalysisDbContext context) : base(context)
        {
        }

    }
}
