using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.BLL.DTOs.Forecast
{
    public record ResultTimeSeriesForecastDto
    {
        public DateTime Date { get; set; }
        public double PredictedValue { get; set; }
    }
}
