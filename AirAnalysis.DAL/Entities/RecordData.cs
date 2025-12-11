using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.DAL.Entities
{
    public class RecordData
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public DateTime DateRecord { get; set; }
        public int? PhenomenId { get; set; } = null!;
        public int? PlaceId { get; set; } = null!;
        public Phenomen? Phenomen { get; set; } = null!;
        public Place? Place { get; set; } = null!;

    }
}
