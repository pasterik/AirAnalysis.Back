using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.BLL.DTOs.RecordData
{
    public record PhenomenByDateDto
    {
        public string Name { get; init; } = null!;
        public List<RecordDateDto> Records { get; init; } = new();
    }
}
