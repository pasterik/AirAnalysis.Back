using AirAnalysis.BLL.DTOs.Phenomen;
using AirAnalysis.BLL.Queries.Ml.Anomaly.GetList;
using AirAnalysis.BLL.Queries.Ml.Forecast.GetList;
using AirAnalysis.BLL.Queries.Phenomen.GetAll;
using AirAnalysis.BLL.Services.MLService;
using AirAnalysis.DAL.Entities;
using AirAnalysis.DAL.Options;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AirAnalysis.Controllers
{
    public class MlController : BaseApiController
    {
        [HttpGet("forecast")]
        public async Task<IActionResult> Forecast([FromQuery] GetForecastQuery query)
        {
            var result = await Mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }

        [HttpGet("anomaly")]
        public async Task<IActionResult> Anomaly([FromQuery] GetAnomalyQuery query)
        {
            var result = await Mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
    }
}
