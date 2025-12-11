// Встановіть NuGet пакети:
// dotnet add package xunit --version 2.6.2
// dotnet add package xunit.runner.visualstudio --version 2.5.4
// dotnet add package Moq --version 4.20.70
// dotnet add package Microsoft.NET.Test.Sdk --version 17.8.0

// Переконайтесь що ваш .csproj містить:
// <IsTestProject>true</IsTestProject>

using AirAnalysis.BLL.Commands.RecordData.AddSeveEco;
using AirAnalysis.BLL.DTOs.RecordData;
using AirAnalysis.DAL.Entities;
using AirAnalysis.DAL.Options;
using AirAnalysis.DAL.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;
using Xunit;
using AirAnalysis.DAL.Repositories;

namespace AirAnalysis.Tests.BLL.Commands.RecordData
{
    public class AddSeveEcoRecordDataCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRecordDataRepository> _recordDataRepoMock;
        private readonly AddSeveEcoRecordDataCommandHandler _handler;

        public AddSeveEcoRecordDataCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _recordDataRepoMock = new Mock<IRecordDataRepository>();

            _unitOfWorkMock.Setup(u => u.RecordData).Returns(_recordDataRepoMock.Object);

            _handler = new AddSeveEcoRecordDataCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidData_ShouldProcessRecordsSuccessfully()
        {
            // Arrange
            var csvContent = @"device_id,logged_at,phenomenon,value,value_text
1,2024-01-15 10:00:00,pm1,15.5,
1,2024-01-15 10:30:00,pm1,16.2,
1,2024-01-15 10:45:00,pm1,14.8,
1,2024-01-15 11:00:00,pm10,25.3,";

            var file = CreateMockFormFile(csvContent);
            var command = new AddSeveEcoRecordDataCommand(1, file);

            _recordDataRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<DAL.Entities.RecordData>>()))
                .ReturnsAsync(new List<DAL.Entities.RecordData>());

            _mapperMock
                .Setup(m => m.Map<DAL.Entities.RecordData>(It.IsAny<CreateRecordDateDto>()))
                .Returns((CreateRecordDateDto dto) => new DAL.Entities.RecordData
                {
                    Value = dto.Value,
                    DateRecord = dto.DateRecord,
                    PhenomenId = dto.PhenomenId,
                    PlaceId = dto.PlaceId
                });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Contains("2 new records", result);
            _recordDataRepoMock.Verify(r => r.CreateRangeAsync(It.IsAny<List<DAL.Entities.RecordData>>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithDuplicateRecords_ShouldSkipExisting()
        {
            // Arrange
            var csvContent = @"device_id,logged_at,phenomenon,value,value_text
1,2024-01-15 10:00:00,pm1,15.5,
1,2024-01-15 11:00:00,pm10,25.3,";

            var file = CreateMockFormFile(csvContent);
            var command = new AddSeveEcoRecordDataCommand(1, file);

            var existingRecords = new List<DAL.Entities.RecordData>
            {
                new DAL.Entities.RecordData
                {
                    DateRecord = new DateTime(2024, 1, 15, 10, 0, 0),
                    PhenomenId = 1,
                    PlaceId = 1,
                    Value = 15.5
                }
            };

            _recordDataRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<DAL.Entities.RecordData>>()))
                .ReturnsAsync(existingRecords);

            _mapperMock
                .Setup(m => m.Map<DAL.Entities.RecordData>(It.IsAny<CreateRecordDateDto>()))
                .Returns((CreateRecordDateDto dto) => new DAL.Entities.RecordData
                {
                    Value = dto.Value,
                    DateRecord = dto.DateRecord,
                    PhenomenId = dto.PhenomenId,
                    PlaceId = dto.PlaceId
                });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Contains("1 new records", result);
            _recordDataRepoMock.Verify(
                r => r.CreateRangeAsync(It.Is<List<DAL.Entities.RecordData>>(list => list.Count == 1)),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WithNullValues_ShouldSkipInvalidRecords()
        {
            // Arrange
            var csvContent = @"device_id,logged_at,phenomenon,value,value_text
1,2024-01-15 10:00:00,pm1,,
1,2024-01-15 11:00:00,pm10,25.3,";

            var file = CreateMockFormFile(csvContent);
            var command = new AddSeveEcoRecordDataCommand(1, file);

            _recordDataRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<DAL.Entities.RecordData>>()))
                .ReturnsAsync(new List<DAL.Entities.RecordData>());

            _mapperMock
                .Setup(m => m.Map<DAL.Entities.RecordData>(It.IsAny<CreateRecordDateDto>()))
                .Returns((CreateRecordDateDto dto) => new DAL.Entities.RecordData
                {
                    Value = dto.Value,
                    DateRecord = dto.DateRecord,
                    PhenomenId = dto.PhenomenId,
                    PlaceId = dto.PlaceId
                });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Contains("1 new records", result);
        }

        [Fact]
        public async Task Handle_WithUnknownPhenomenon_ShouldSkipRecord()
        {
            // Arrange
            var csvContent = @"device_id,logged_at,phenomenon,value,value_text
1,2024-01-15 10:00:00,unknown_phenomenon,15.5,
1,2024-01-15 11:00:00,pm10,25.3,";

            var file = CreateMockFormFile(csvContent);
            var command = new AddSeveEcoRecordDataCommand(1, file);

            _recordDataRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<DAL.Entities.RecordData>>()))
                .ReturnsAsync(new List<DAL.Entities.RecordData>());

            _mapperMock
                .Setup(m => m.Map<DAL.Entities.RecordData>(It.IsAny<CreateRecordDateDto>()))
                .Returns((CreateRecordDateDto dto) => new DAL.Entities.RecordData
                {
                    Value = dto.Value,
                    DateRecord = dto.DateRecord,
                    PhenomenId = dto.PhenomenId,
                    PlaceId = dto.PlaceId
                });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Contains("1 new records", result);
        }

        [Fact]
        public async Task Handle_WithAveragingMultipleValuesPerHour_ShouldCalculateCorrectAverage()
        {
            // Arrange
            var csvContent = @"device_id,logged_at,phenomenon,value,value_text
1,2024-01-15 10:10:00,pm1,10.0,
1,2024-01-15 10:20:00,pm1,20.0,
1,2024-01-15 10:30:00,pm1,30.0,";

            var file = CreateMockFormFile(csvContent);
            var command = new AddSeveEcoRecordDataCommand(1, file);

            DAL.Entities.RecordData capturedRecord = null;

            _recordDataRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<DAL.Entities.RecordData>>()))
                .ReturnsAsync(new List<DAL.Entities.RecordData>());

            _mapperMock
                .Setup(m => m.Map<DAL.Entities.RecordData>(It.IsAny<CreateRecordDateDto>()))
                .Returns((CreateRecordDateDto dto) =>
                {
                    capturedRecord = new DAL.Entities.RecordData
                    {
                        Value = dto.Value,
                        DateRecord = dto.DateRecord,
                        PhenomenId = dto.PhenomenId,
                        PlaceId = dto.PlaceId
                    };
                    return capturedRecord;
                });

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedRecord);
            Assert.Equal(20.0, capturedRecord.Value); // Average of 10, 20, 30
            Assert.Equal(new DateTime(2024, 1, 15, 10, 0, 0), capturedRecord.DateRecord);
        }

        [Fact]
        public async Task Handle_WithEmptyPhenomenon_ShouldSkipRecord()
        {
            // Arrange
            var csvContent = @"device_id,logged_at,phenomenon,value,value_text
1,2024-01-15 10:00:00,,15.5,
1,2024-01-15 11:00:00,pm10,25.3,";

            var file = CreateMockFormFile(csvContent);
            var command = new AddSeveEcoRecordDataCommand(1, file);

            _recordDataRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<DAL.Entities.RecordData>>()))
                .ReturnsAsync(new List<DAL.Entities.RecordData>());

            _mapperMock
                .Setup(m => m.Map<DAL.Entities.RecordData>(It.IsAny<CreateRecordDateDto>()))
                .Returns((CreateRecordDateDto dto) => new DAL.Entities.RecordData
                {
                    Value = dto.Value,
                    DateRecord = dto.DateRecord,
                    PhenomenId = dto.PhenomenId,
                    PlaceId = dto.PlaceId
                });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Contains("1 new records", result);
        }

        [Fact]
        public async Task Handle_WithAllPhenomenonTypes_ShouldMapCorrectly()
        {
            // Arrange
            var csvContent = @"device_id,logged_at,phenomenon,value,value_text
1,2024-01-15 10:00:00,pm1,10.0,
1,2024-01-15 11:00:00,pm10,20.0,
1,2024-01-15 12:00:00,pm25,30.0,
1,2024-01-15 13:00:00,co_ug,40.0,
1,2024-01-15 14:00:00,o3_ug,50.0,
1,2024-01-15 15:00:00,no2_ug,60.0,
1,2024-01-15 16:00:00,so2_ug,70.0,";

            var file = CreateMockFormFile(csvContent);
            var command = new AddSeveEcoRecordDataCommand(1, file);

            var capturedRecords = new List<DAL.Entities.RecordData>();

            _recordDataRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<DAL.Entities.RecordData>>()))
                .ReturnsAsync(new List<DAL.Entities.RecordData>());

            _mapperMock
                .Setup(m => m.Map<DAL.Entities.RecordData>(It.IsAny<CreateRecordDateDto>()))
                .Returns((CreateRecordDateDto dto) =>
                {
                    var record = new DAL.Entities.RecordData
                    {
                        Value = dto.Value,
                        DateRecord = dto.DateRecord,
                        PhenomenId = dto.PhenomenId,
                        PlaceId = dto.PlaceId
                    };
                    capturedRecords.Add(record);
                    return record;
                });

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(7, capturedRecords.Count);
            Assert.Contains(capturedRecords, r => r.PhenomenId == 1); // pm1
            Assert.Contains(capturedRecords, r => r.PhenomenId == 2); // pm10
            Assert.Contains(capturedRecords, r => r.PhenomenId == 3); // pm25
            Assert.Contains(capturedRecords, r => r.PhenomenId == 4); // co_ug
            Assert.Contains(capturedRecords, r => r.PhenomenId == 5); // o3_ug
            Assert.Contains(capturedRecords, r => r.PhenomenId == 6); // no2_ug
            Assert.Contains(capturedRecords, r => r.PhenomenId == 7); // so2_ug
        }

        [Fact]
        public async Task Handle_WithLargeDataSet_ShouldProcessInBatches()
        {
            // Arrange
            var csvLines = new List<string> { "device_id,logged_at,phenomenon,value,value_text" };
            for (int i = 0; i < 2500; i++)
            {
                csvLines.Add($"1,2024-01-15 {i % 24:D2}:00:00,pm1,{10.0 + i},");
            }
            var csvContent = string.Join("\n", csvLines);

            var file = CreateMockFormFile(csvContent);
            var command = new AddSeveEcoRecordDataCommand(1, file);

            _recordDataRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<DAL.Entities.RecordData>>()))
                .ReturnsAsync(new List<DAL.Entities.RecordData>());

            _mapperMock
                .Setup(m => m.Map<DAL.Entities.RecordData>(It.IsAny<CreateRecordDateDto>()))
                .Returns((CreateRecordDateDto dto) => new DAL.Entities.RecordData
                {
                    Value = dto.Value,
                    DateRecord = dto.DateRecord,
                    PhenomenId = dto.PhenomenId,
                    PlaceId = dto.PlaceId
                });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _recordDataRepoMock.Verify(
                r => r.CreateRangeAsync(It.IsAny<List<DAL.Entities.RecordData>>()),
                Times.AtLeastOnce
            );
            _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_WithInvalidDateFormat_ShouldSkipRecord()
        {
            // Arrange
            var csvContent = @"device_id,logged_at,phenomenon,value,value_text
1,invalid-date,pm1,15.5,
1,2024-01-15 11:00:00,pm10,25.3,";

            var file = CreateMockFormFile(csvContent);
            var command = new AddSeveEcoRecordDataCommand(1, file);

            _recordDataRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<DAL.Entities.RecordData>>()))
                .ReturnsAsync(new List<DAL.Entities.RecordData>());

            _mapperMock
                .Setup(m => m.Map<DAL.Entities.RecordData>(It.IsAny<CreateRecordDateDto>()))
                .Returns((CreateRecordDateDto dto) => new DAL.Entities.RecordData
                {
                    Value = dto.Value,
                    DateRecord = dto.DateRecord,
                    PhenomenId = dto.PhenomenId,
                    PlaceId = dto.PlaceId
                });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Contains("1 new records", result);
        }

        private IFormFile CreateMockFormFile(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.FileName).Returns("test.csv");
            fileMock.Setup(f => f.Length).Returns(stream.Length);

            return fileMock.Object;
        }
    }
}