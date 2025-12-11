using AirAnalysis.BLL.DTOs.Phenomen;
using AirAnalysis.BLL.Queries.Phenomen.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AirAnalysis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private IMediator? _mediator;

        protected IMediator Mediator => _mediator ??=
            HttpContext.RequestServices.GetService<IMediator>()!;
    }
}
