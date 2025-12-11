using AirAnalysis.BLL.DTOs.Anomaly;
using AirAnalysis.BLL.DTOs.Forecast;
using AirAnalysis.BLL.DTOs.Phenomen;
using AirAnalysis.BLL.Services.MLService;
using AirAnalysis.DAL.Data;
using AirAnalysis.DAL.Entities;
using AirAnalysis.DAL.Options;
using AirAnalysis.DAL.Repositories.Interfaces;
using AirAnalysis.DAL.Repositories.Realizations;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AirAnalysis.BLL.Queries.Ml.Anomaly.GetList
{
    public class GetAnomalyHandler : IRequestHandler<GetAnomalyQuery, Result<AnomalyReportDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MlService _mlService;
        private readonly AirAnalysisDbContext _context;

        public GetAnomalyHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            MlService mlService,
            AirAnalysisDbContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mlService = mlService;
            _context = context;
        }

        public async Task<Result<AnomalyReportDto>> Handle(GetAnomalyQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var phenomenId = request.forecast.PhenomenIds;
                var placeId = request.forecast.PlaceIds;
                DateTime dataStart = request.forecast.DataStart;
                DateTime dataEnd = request.forecast.DataEnd;

                if (dataStart > dataEnd)
                {
                    return new Result<AnomalyReportDto>
                    {
                        Error = "Start date (DataStart) cannot be later than end date (DataEnd)."
                    };
                }
                if (!_mlService.IsAnomalyModelAvailable(phenomenId))
                {
                    return new Result<AnomalyReportDto>
                    {
                        Error = $"Anomaly model not found for PhenomenId={phenomenId}"
                    };
                }
                var viewRecords = await _context.RecordDataDailyView
                    .AsNoTracking()
                    .Where(v => v.PhenomenId == phenomenId &&
                               v.PlaceId == placeId &&
                               v.Date >= dataStart.Date &&
                               v.Date <= dataEnd.Date)
                    .OrderBy(v => v.Date)
                    .ToListAsync(cancellationToken);

                if (viewRecords == null || !viewRecords.Any())
                {
                    return new Result<AnomalyReportDto>
                    {
                        Error = $"No records found for PhenomenId={phenomenId}, PlaceId={placeId} in date range {dataStart:yyyy-MM-dd} to {dataEnd:yyyy-MM-dd}"
                    };
                }

                var results = viewRecords
                    .Select(v =>
                    {
                        var avgValue = v.AvgValue ?? 0;
                        var anomalyResult = _mlService.CheckAnomalyWithMetadata(avgValue, phenomenId);

                        return new AnomalyRecordDto
                        {
                            Date = v.Date.ToString("yyyy-MM-dd"),
                            Value = Math.Round(avgValue, 2),
                            IsAnomaly = anomalyResult.IsAnomaly,
                            Mean = anomalyResult.Mean.HasValue
                                ? Math.Round(anomalyResult.Mean.Value, 2)
                                : null,
                            Std = anomalyResult.Std.HasValue
                                ? Math.Round(anomalyResult.Std.Value, 2)
                                : null,
                            Deviation = anomalyResult.DeviationFromMean.HasValue
                                ? Math.Round(anomalyResult.DeviationFromMean.Value, 2)
                                : null,
                            ZScore = anomalyResult.ZScore.HasValue
                                ? Math.Round(anomalyResult.ZScore.Value, 2)
                                : null
                        };
                    })
                    .ToList();

                var anomalyCount = results.Count(r => r.IsAnomaly);

                return new Result<AnomalyReportDto>
                {
                    Data = new AnomalyReportDto
                    {
                        PhenomenId = phenomenId,
                        PlaceId = placeId,
                        TotalRecords = results.Count,
                        AnomalyCount = anomalyCount,
                        AnomalyPercentage = results.Count > 0
                            ? Math.Round((double)anomalyCount / results.Count * 100, 2)
                            : 0,
                        Records = results
                    }
                };
            }
            catch (FileNotFoundException ex)
            {
                return new Result<AnomalyReportDto>
                {
                    Error = $"Model file not found: {ex.Message}"
                };
            }
            catch (ArgumentException ex)
            {
                return new Result<AnomalyReportDto>
                {
                    Error = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Result<AnomalyReportDto>
                {
                    Error = $"Unexpected error during anomaly detection: {ex.Message}"
                };
            }
        }
    }
}