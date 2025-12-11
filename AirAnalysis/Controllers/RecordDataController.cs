using AirAnalysis.BLL.Commands.RecordData.AddSeveEco;
using AirAnalysis.BLL.DTOs.Phenomen;
using AirAnalysis.BLL.DTOs.RecordData;
using AirAnalysis.BLL.Queries.Phenomen.GetAll;
using AirAnalysis.BLL.Queries.RecordData.GetByData;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AirAnalysis.Controllers
{
    public class RecordDataController : BaseApiController
    {
        [HttpPost("upload-csv")]
        [RequestSizeLimit(2147483648)]
        public async Task<IActionResult> UploadCsv([FromForm] AddSeveEcoRecordDataCommand request)
        {
            var result = await Mediator.Send(request);
            return Ok(result);
        }
        [HttpPost("by-date")]
        public async Task<ActionResult<GetByDateResponseDto>> GetByDate([FromForm] GetByDateRecordDataQuery request)
        {
            var result = await Mediator.Send(request);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result);
        }
    }
}
