using AirAnalysis.BLL.DTOs.Forecast;
using AirAnalysis.BLL.DTOs.Filter;

using MediatR;

namespace AirAnalysis.BLL.Queries.Ml.Forecast.GetList
{
    public record GetForecastQuery(FilterTimeSeriesForecastDto forecast) : IRequest<Result<List<TimeSeriesForecastDto>>>;
}
