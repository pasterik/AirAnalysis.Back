using AirAnalysis.BLL.DTOs.CsvRecord;
using AirAnalysis.BLL.DTOs.RecordData;
using AirAnalysis.DAL.Options;
using AirAnalysis.DAL.Repositories.Interfaces;
using AutoMapper;
using CsvHelper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AirAnalysis.BLL.Commands.RecordData.AddSeveEco
{
    public class AddSeveEcoRecordDataCommandHandler : IRequestHandler<AddSeveEcoRecordDataCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AddSeveEcoRecordDataCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<string> Handle(AddSeveEcoRecordDataCommand request, CancellationToken cancellationToken)
        {
            using var stream = request.file.OpenReadStream();
            using var reader = new StreamReader(stream);
            var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null,
                MissingFieldFound = null,
            };
            using var csv = new CsvReader(reader, config);
            csv.Context.TypeConverterCache.AddConverter<DateTime?>(new NullableDateTimeConverter());
            var records = csv.GetRecords<SaveBot>().ToList();

            var filteredRecords = records
                .Where(c => c.logged_at.HasValue && !string.IsNullOrWhiteSpace(c.phenomenon))
                .GroupBy(c => new
                {
                    Date = c.logged_at.Value.Date,
                    Hour = c.logged_at.Value.Hour,
                    c.phenomenon
                });

            var dtos = filteredRecords
                .Select(g => new
                {
                    Value = g.Average(x => x.value),
                    Date = new DateTime(g.Key.Date.Year, g.Key.Date.Month, g.Key.Date.Day, g.Key.Hour, 0, 0),
                    PhenomenId = MapPhenomenon(g.Key.phenomenon),
                })
                .Where(x => x.Value.HasValue && x.PhenomenId != -1)
                .Select(x => new
                {
                    Value = x.Value.Value,
                    x.Date,
                    x.PhenomenId
                })
                .ToList();

            var dateHours = dtos.Select(x => x.Date).Distinct().ToList();
            var phenomenIds = dtos.Select(x => x.PhenomenId).Distinct().ToList();

            var existingRecords = await _unitOfWork.RecordData
                .GetAllAsync(new QueryOptions<DAL.Entities.RecordData>
                {
                    Filter = rd => rd.PlaceId == request.idPlace &&
                                  dateHours.Contains(rd.DateRecord) &&
                                  rd.PhenomenId.HasValue &&
                                  phenomenIds.Contains(rd.PhenomenId.Value)
                });

            var existingKeys = existingRecords
                .Where(r => r.PhenomenId.HasValue)
                .Select(r => $"{r.DateRecord:yyyy-MM-dd-HH}_{r.PhenomenId.Value}")
                .ToHashSet();

            var newDataList = new List<DAL.Entities.RecordData>();

            foreach (var item in dtos)
            {
                var key = $"{item.Date:yyyy-MM-dd-HH}_{item.PhenomenId}";
                if (existingKeys.Contains(key)) continue;

                var newRecord = _mapper.Map<DAL.Entities.RecordData>(new CreateRecordDateDto
                {
                    Value = item.Value,
                    DateRecord = item.Date,
                    PhenomenId = item.PhenomenId,
                    PlaceId = request.idPlace
                });

                newDataList.Add(newRecord);
            }

            const int batchSize = 1000;
            int totalProcessed = 0;

            for (int i = 0; i < newDataList.Count; i += batchSize)
            {
                var batch = newDataList.Skip(i).Take(batchSize).ToList();
                await _unitOfWork.RecordData.CreateRangeAsync(batch);
                await _unitOfWork.SaveAsync();
                totalProcessed += batch.Count;
            }

            return $"Processed {totalProcessed} new records successfully";
        }
        private static int MapPhenomenon(string name)
        {
            return name switch
            {
                "pm1" => 1,
                "pm10" => 2,
                "pm25" => 3,
                "co_ug" => 4,
                "o3_ug" => 5,
                "no2_ug" => 6,
                "so2_ug" => 7,
                _ => -1
            };
        }
    }
    public class NullableDateTimeConverter : CsvHelper.TypeConversion.DefaultTypeConverter
    {
        public override object? ConvertFromString(string text, IReaderRow row, CsvHelper.Configuration.MemberMapData memberMapData)
        {
            if (DateTime.TryParse(text, out var date))
                return date;
            return null;
        }
    }
}
