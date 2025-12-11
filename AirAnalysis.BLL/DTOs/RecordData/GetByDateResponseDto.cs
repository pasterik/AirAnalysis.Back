using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.BLL.DTOs.RecordData
{
    public record GetByDateResponseDto
    {
        public List<PhenomenByDateDto> Phenomens { get; init; } = new();
        public AirQualityAssessmentDto? AirQualityAssessment { get; init; }
    }
}
