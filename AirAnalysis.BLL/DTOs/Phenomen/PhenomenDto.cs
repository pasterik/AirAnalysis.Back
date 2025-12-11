using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.BLL.DTOs.Phenomen
{
    public record PhenomenDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
