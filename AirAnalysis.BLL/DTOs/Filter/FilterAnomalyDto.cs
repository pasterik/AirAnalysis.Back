using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.BLL.DTOs.Filter
{
    public record FilterAnomalyDto
    {
        public int PhenomenIds { get; set; }
        public int PlaceIds { get; set; }
        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }
    }
}
