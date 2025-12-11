// Queries/RecordData/GetByData/GetByGetDataRecordDataQuery.cs
using AirAnalysis.BLL.DTOs.RecordData;
using AirAnalysis.BLL.Services.Fuzzy_LogicService.Interfaces;
using AirAnalysis.BLL.Services.Fuzzy_LogicService.Model;
using AirAnalysis.DAL.Options;
using AirAnalysis.DAL.Repositories.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AirAnalysis.BLL.Queries.RecordData.GetByData
{
    public class GetByGetDataRecordDataQuery : IRequestHandler<GetByDateRecordDataQuery, Result<GetByDateResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFuzzy_Logic _fuzzyLogic;

        public GetByGetDataRecordDataQuery(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IFuzzy_Logic fuzzyLogic)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fuzzyLogic = fuzzyLogic;
        }

        public async Task<Result<GetByDateResponseDto>> Handle(GetByDateRecordDataQuery request, CancellationToken cancellationToken)
        {
            var options = new QueryOptions<DAL.Entities.Phenomen>
            {
                AsNoTracking = true,
                Include = q => q.Include(p => p.RecordDatas
                    .Where(r => r.DateRecord.Date == request.date && r.PlaceId == request.placeId)),
                Filter = p => p.RecordDatas.Any(r => r.DateRecord.Date == request.date && r.PlaceId == request.placeId)
            };

            try
            {
                var phenomenList = await _unitOfWork.Phenomen.GetAllAsync(options);
                var phenomenDtoList = _mapper.Map<List<PhenomenByDateDto>>(phenomenList);

                if (phenomenDtoList == null || !phenomenDtoList.Any())
                {
                    return new Result<GetByDateResponseDto>
                    {
                        Error = $"No records found for Date={request.date}, PlaceId={request.placeId}"
                    };
                }

                var pollutants = CalculateAveragePollutants(phenomenDtoList);

                AirQualityAssessmentDto? assessment = null;
                if (pollutants.Any())
                {
                    assessment = CalculateAirQualityAssessment(pollutants);
                }

                return new Result<GetByDateResponseDto>
                {
                    Data = new GetByDateResponseDto
                    {
                        Phenomens = phenomenDtoList,
                        AirQualityAssessment = assessment
                    }
                };
            }
            catch (Exception ex)
            {
                return new Result<GetByDateResponseDto>
                {
                    Error = ex.Message
                };
            }
        }

        private Dictionary<string, double> CalculateAveragePollutants(List<PhenomenByDateDto> phenomenList)
        {
            var pollutants = new Dictionary<string, double>();

            foreach (var phenomen in phenomenList)
            {
                if (phenomen.Records.Any())
                {
                    var avgValue = phenomen.Records.Average(r => r.Value);
                    pollutants[phenomen.Name.ToLower()] = avgValue;
                }
            }

            return pollutants;
        }

        private AirQualityAssessmentDto CalculateAirQualityAssessment(Dictionary<string, double> pollutants)
        {
            var phenomenFuzzy = new PhenomenFuzzy
            {
                PM1 = pollutants.GetValueOrDefault("pm1", 1),
                PM2_5 = pollutants.GetValueOrDefault("pm25", 1),
                PM10 = pollutants.GetValueOrDefault("pm10", 1),
                CO = pollutants.GetValueOrDefault("co", 1),
                O3 = pollutants.GetValueOrDefault("o3", 1),
                NO2 = pollutants.GetValueOrDefault("no2", 1),
                SO2 = pollutants.GetValueOrDefault("so2", 1)
            };

            var rules = _fuzzyLogic.GetRule();

            var fuzzyScore = _fuzzyLogic.GetResult(phenomenFuzzy, rules);

            var category = GetCategoryFromScore(fuzzyScore);

            return new AirQualityAssessmentDto
            {
                FuzzyScore = Math.Round(fuzzyScore, 2),
                Category = category
            };
        }

        private string GetCategoryFromScore(double score)
        {
            return score switch
            {
                <= 4.0 => "Дуже добре",
                <= 7.0 => "Добре",
                <= 9.0 => "Нормально",
                < 10.0 => "Погано",
                _ => "Дуже погано"
            };
        }
    }
}