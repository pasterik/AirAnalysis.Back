using AirAnalysis.BLL.DTOs.Phenomen;
using AirAnalysis.BLL.Queries.Phenomen.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AirAnalysis.Controllers
{

    public class PhenomenController : BaseApiController
    {

        [HttpGet]
        public async Task<ActionResult<List<PhenomenDto>>> GetAll()
        {
            return await Mediator.Send(new GetAllPhenomenQuery());
        }
    }
}
