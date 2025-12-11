using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AirAnalysis.DAL.Entities
{
    public class Phenomen
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<RecordData>? RecordDatas { get; set; } = [];

    }
}
