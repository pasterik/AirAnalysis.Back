using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.BLL.DTOs.RecordData
{
    public record CreateRecordDateDto
    {
        public double Value { get; set; }
        public DateTime DateRecord { get; set; }
        public int? PhenomenId { get; set; } = null!;
        public int? PlaceId { get; set; } = null!;
    }
}
