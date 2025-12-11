using AirAnalysis.BLL.DTOs.RecordData;
using MediatR;

namespace AirAnalysis.BLL.Queries.RecordData.GetByData
{
    public record GetByDateRecordDataQuery(int placeId, DateTime date) : IRequest<Result<GetByDateResponseDto>>;
}