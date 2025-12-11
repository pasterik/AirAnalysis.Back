using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.BLL.DTOs.Forecast
{
    public record TimeSeriesForecastDto
    {
        public int PhenomenId { get; set; }
        public int PlaceId { get; set; }
        public DateTime LastDate { get; set; }
        public List<ResultTimeSeriesForecastDto> Forecasts { get; set; }
    }
}
