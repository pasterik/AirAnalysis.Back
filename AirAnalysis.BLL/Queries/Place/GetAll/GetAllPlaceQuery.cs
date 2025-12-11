using AirAnalysis.BLL.DTOs.Place;
using MediatR;

namespace AirAnalysis.BLL.Queries.Place.GetAll
{
    public record GetAllPlaceQuery() : IRequest<List<PlaceDto>>;
}
