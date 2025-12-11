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

namespace AirAnalysis.BLL.Queries.Ml.Forecast.GetList
{
    public class GetForecastHandler : IRequestHandler<GetForecastQuery, Result<List<TimeSeriesForecastDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MlService _mlService;
        private readonly AirAnalysisDbContext _context;

        public GetForecastHandler(
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

        public async Task<Result<List<TimeSeriesForecastDto>>> Handle(GetForecastQuery request, CancellationToken cancellationToken)
        {
            try
            {
                int phenomenId = request.forecast.phenomenId;
                int placeId = request.forecast.placeId;
                int days = request.forecast.days;

                // ЗМІНЕНО: перевіряємо тільки phenomenId (без placeId)
                if (!_mlService.IsForecastModelAvailable(phenomenId))
                {
                    return new Result<List<TimeSeriesForecastDto>>
                    {
                        Error = $"Forecast model not found for PhenomenId={phenomenId}. Please train the model first."
                    };
                }

                // Отримуємо дані з View для конкретного місця
                // (модель тренується на всіх місцях, але прогноз робимо для конкретного)
                var viewRecords = await _context.RecordDataDailyView
                    .AsNoTracking()
                    .Where(v => v.PhenomenId == phenomenId && v.PlaceId == placeId)
                    .OrderByDescending(v => v.Date)
                    .Take(100) // Беремо більше даних для кращого контексту
                    .ToListAsync(cancellationToken);

                if (viewRecords == null || viewRecords.Count < 3)
                {
                    return new Result<List<TimeSeriesForecastDto>>
                    {
                        Error = $"Not enough records for forecasting (need at least 3). Found: {viewRecords?.Count ?? 0}"
                    };
                }

                // Сортуємо по даті (від старої до нової) для time series
                var orderedRecords = viewRecords.OrderBy(v => v.Date).ToList();

                // Беремо останні 3 значення для lag features
                var lastThreeRecords = orderedRecords.TakeLast(3).ToList();

                // Використовуємо AvgValue з View
                var lastValues = lastThreeRecords
                    .Select(v => (float)(v.AvgValue ?? 0))
                    .ToList();

                var lastDate = orderedRecords.Max(v => v.Date);

                // ЗМІНЕНО: викликаємо без placeId
                var predictions = _mlService.PredictTimeseries(
                    lastValues,
                    phenomenId,
                    lastDate,
                    days);

                var dto = new TimeSeriesForecastDto
                {
                    PhenomenId = phenomenId,
                    PlaceId = placeId,
                    LastDate = lastDate,
                    Forecasts = predictions.Select(p => new ResultTimeSeriesForecastDto
                    {
                        Date = p.Date,
                        PredictedValue = Math.Round(p.PredictedValue, 2)
                    }).ToList()
                };

                return new Result<List<TimeSeriesForecastDto>>
                {
                    Data = new List<TimeSeriesForecastDto> { dto }
                };
            }
            catch (FileNotFoundException ex)
            {
                return new Result<List<TimeSeriesForecastDto>>
                {
                    Error = $"Model file not found: {ex.Message}"
                };
            }
            catch (ArgumentException ex)
            {
                return new Result<List<TimeSeriesForecastDto>>
                {
                    Error = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Result<List<TimeSeriesForecastDto>>
                {
                    Error = $"Unexpected error during forecasting: {ex.Message}"
                };
            }
        }
    }
}