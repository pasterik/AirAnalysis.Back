using AirAnalysis.BLL.DTOs.Phenomen;
using AirAnalysis.BLL.DTOs.Place;
using AirAnalysis.BLL.Queries.Phenomen.GetAll;
using AirAnalysis.BLL.Queries.Place.GetAll;
using AirAnalysis.DAL.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AirAnalysis.Controllers
{

    public class PlaceController : BaseApiController
    {

        [HttpGet]
        public async Task<ActionResult<List<PlaceDto>>> GetAll()
        {
            return await Mediator.Send(new GetAllPlaceQuery());
        }
    }
}
