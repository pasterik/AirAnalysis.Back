using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.BLL.DTOs.Filter
{
    public record FilterTimeSeriesForecastDto
    {
        public int phenomenId { get; init; }
        public int placeId { get; init; }
        public int days { get; init; }
    }
}
