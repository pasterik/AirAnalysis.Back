using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.DAL.Entities
{
    public class Place
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<RecordData>? RecordDatas { get; set; } = [];
    }
}
