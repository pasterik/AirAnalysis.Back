using AirAnalysis.BLL.DTOs.Phenomen;
using MediatR;

namespace AirAnalysis.BLL.Queries.Phenomen.GetAll
{
    public record GetAllPhenomenQuery() : IRequest<List<PhenomenDto>>;
}
