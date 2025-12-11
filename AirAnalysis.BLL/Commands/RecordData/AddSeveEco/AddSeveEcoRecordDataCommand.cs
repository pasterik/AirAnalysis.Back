using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.BLL.Commands.RecordData.AddSeveEco
{
    public record AddSeveEcoRecordDataCommand(int idPlace, IFormFile file) : IRequest<string>;
}
