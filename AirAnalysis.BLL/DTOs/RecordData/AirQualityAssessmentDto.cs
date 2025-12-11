using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.BLL.DTOs.RecordData
{
    public record AirQualityAssessmentDto
    {
        public double FuzzyScore { get; init; }
        public string Category { get; init; } = string.Empty;
    }
}
