using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.BLL.DTOs.Place
{
    public record PlaceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
