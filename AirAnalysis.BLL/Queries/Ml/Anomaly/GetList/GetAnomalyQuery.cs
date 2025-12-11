using AirAnalysis.BLL.DTOs.Forecast;
using AirAnalysis.BLL.DTOs.Filter;
using MediatR;
using AirAnalysis.BLL.DTOs.Anomaly;

namespace AirAnalysis.BLL.Queries.Ml.Anomaly.GetList
{
    public record GetAnomalyQuery(FilterAnomalyDto forecast) : IRequest<Result<AnomalyReportDto>>;
}
