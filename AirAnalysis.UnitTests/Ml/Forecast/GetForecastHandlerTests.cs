using AirAnalysis.BLL.DTOs.Forecast;
using AirAnalysis.BLL.Queries.Forecast;
using Xunit;

namespace AirAnalysis.UnitTests.Ml.Forecast
{
    public class GetForecastHandlerTests
    {
        [Fact]
        public void GetForecastQuery_ShouldAcceptForecastDto()
        {
            // Arrange
            var forecast = new BLL.DTOs.Filter.FilterTimeSeriesForecastDto
            {
                phenomenId = 1,
                placeId = 2,
                days = 7
            };

            // Act
            var query = new GetForecastQuery(forecast);

            // Assert
            Assert.NotNull(query);
            Assert.Equal(forecast, query.forecast);
        }

        [Fact]
        public void ForecastDto_ShouldStoreCorrectValues()
        {
            // Arrange & Act
            var forecast = new BLL.DTOs.Filter.FilterTimeSeriesForecastDto
            {
                phenomenId = 5,
                placeId = 10,
                days = 14
            };

            // Assert
            Assert.Equal(5, forecast.phenomenId);
            Assert.Equal(10, forecast.placeId);
            Assert.Equal(14, forecast.days);
        }

        [Fact]
        public void ForecastDto_WithDifferentPhenomenIds_ShouldBeDistinct()
        {
            // Arrange
            var forecast1 = new BLL.DTOs.Filter.FilterTimeSeriesForecastDto { phenomenId = 1, placeId = 1, days = 7 };
            var forecast2 = new BLL.DTOs.Filter.FilterTimeSeriesForecastDto { phenomenId = 2, placeId = 1, days = 7 };

            // Assert
            Assert.NotEqual(forecast1.phenomenId, forecast2.phenomenId);
        }

        [Fact]
        public void ForecastDto_WithDifferentPlaceIds_ShouldBeDistinct()
        {
            // Arrange
            var forecast1 = new BLL.DTOs.Filter.FilterTimeSeriesForecastDto { phenomenId = 1, placeId = 1, days = 7 };
            var forecast2 = new BLL.DTOs.Filter.FilterTimeSeriesForecastDto { phenomenId = 1, placeId = 2, days = 7 };

            // Assert
            Assert.NotEqual(forecast1.placeId, forecast2.placeId);
        }

        [Fact]
        public void ForecastDto_WithDifferentDays_ShouldBeDistinct()
        {
            // Arrange
            var forecast1 = new BLL.DTOs.Filter.FilterTimeSeriesForecastDto { phenomenId = 1, placeId = 1, days = 7 };
            var forecast2 = new BLL.DTOs.Filter.FilterTimeSeriesForecastDto { phenomenId = 1, placeId = 1, days = 14 };

            // Assert
            Assert.NotEqual(forecast1.days, forecast2.days);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(2, 5, 7)]
        [InlineData(3, 10, 14)]
        [InlineData(4, 20, 30)]
        public void ForecastDto_ShouldAcceptDifferentValues(int phenomenId, int placeId, int days)
        {
            // Arrange & Act
            var forecast = new BLL.DTOs.Filter.FilterTimeSeriesForecastDto
            {
                phenomenId = phenomenId,
                placeId = placeId,
                days = days
            };

            // Assert
            Assert.Equal(phenomenId, forecast.phenomenId);
            Assert.Equal(placeId, forecast.placeId);
            Assert.Equal(days, forecast.days);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(7)]
        [InlineData(14)]
        [InlineData(30)]
        [InlineData(90)]
        public void ForecastDto_WithVariousDayValues_ShouldAcceptAll(int days)
        {
            // Arrange & Act
            var forecast = new BLL.DTOs.Filter.FilterTimeSeriesForecastDto
            {
                phenomenId = 1,
                placeId = 1,
                days = days
            };

            // Assert
            Assert.Equal(days, forecast.days);
            Assert.True(forecast.days > 0);
        }

        [Fact]
        public void ForecastDto_WithPositiveDays_ShouldBeValid()
        {
            // Arrange & Act
            var forecast = new BLL.DTOs.Filter.FilterTimeSeriesForecastDto
            {
                phenomenId = 1,
                placeId = 1,
                days = 7
            };

            // Assert
            Assert.True(forecast.days > 0);
        }

        [Fact]
        public void GetForecastQuery_WithMultipleCreations_ShouldBeIndependent()
        {
            // Arrange
            var forecast1 = new BLL.DTOs.Filter.FilterTimeSeriesForecastDto
            {
                phenomenId = 1,
                placeId = 1,
                days = 7
            };

            var forecast2 = new BLL.DTOs.Filter.FilterTimeSeriesForecastDto
            {
                phenomenId = 2,
                placeId = 2,
                days = 14
            };

            // Act
            var query1 = new GetForecastQuery(forecast1);
            var query2 = new GetForecastQuery(forecast2);

            // Assert
            Assert.NotEqual(query1.forecast.phenomenId, query2.forecast.phenomenId);
            Assert.NotEqual(query1.forecast.placeId, query2.forecast.placeId);
            Assert.NotEqual(query1.forecast.days, query2.forecast.days);
        }

        [Fact]
        public void TimeSeriesForecastDto_ShouldStoreCorrectValues()
        {
            // Arrange & Act
            var dto = new TimeSeriesForecastDto
            {
                PhenomenId = 1,
                PlaceId = 2,
                LastDate = new DateTime(2024, 1, 15),
                Forecasts = new List<ResultTimeSeriesForecastDto>
                {
                    new ResultTimeSeriesForecastDto { Date = new DateTime(2024, 1, 16), PredictedValue = 15.5 },
                    new ResultTimeSeriesForecastDto { Date = new DateTime(2024, 1, 17), PredictedValue = 16.2 }
                }
            };

            // Assert
            Assert.Equal(1, dto.PhenomenId);
            Assert.Equal(2, dto.PlaceId);
            Assert.Equal(new DateTime(2024, 1, 15), dto.LastDate);
            Assert.Equal(2, dto.Forecasts.Count);
        }

        [Fact]
        public void ResultTimeSeriesForecastDto_ShouldStoreCorrectValues()
        {
            // Arrange & Act
            var result = new ResultTimeSeriesForecastDto
            {
                Date = new DateTime(2024, 1, 20),
                PredictedValue = 25.75
            };

            // Assert
            Assert.Equal(new DateTime(2024, 1, 20), result.Date);
            Assert.Equal(25.75, result.PredictedValue);
        }

        [Fact]
        public void TimeSeriesForecastDto_WithEmptyForecasts_ShouldBeValid()
        {
            // Arrange & Act
            var dto = new TimeSeriesForecastDto
            {
                PhenomenId = 1,
                PlaceId = 1,
                LastDate = DateTime.Now,
                Forecasts = new List<ResultTimeSeriesForecastDto>()
            };

            // Assert
            Assert.NotNull(dto.Forecasts);
            Assert.Empty(dto.Forecasts);
        }

        [Fact]
        public void TimeSeriesForecastDto_WithMultipleForecasts_ShouldStoreAll()
        {
            // Arrange
            var forecasts = new List<ResultTimeSeriesForecastDto>();
            for (int i = 1; i <= 7; i++)
            {
                forecasts.Add(new ResultTimeSeriesForecastDto
                {
                    Date = new DateTime(2024, 1, i),
                    PredictedValue = 10.0 + i
                });
            }

            // Act
            var dto = new TimeSeriesForecastDto
            {
                PhenomenId = 1,
                PlaceId = 1,
                LastDate = new DateTime(2024, 1, 1),
                Forecasts = forecasts
            };

            // Assert
            Assert.Equal(7, dto.Forecasts.Count);
            Assert.Equal(11.0, dto.Forecasts[0].PredictedValue);
            Assert.Equal(17.0, dto.Forecasts[6].PredictedValue);
        }

        [Theory]
        [InlineData(10.123, 10.12)]
        [InlineData(15.678, 15.68)]
        [InlineData(20.999, 21.0)]
        [InlineData(25.001, 25.0)]
        public void ResultTimeSeriesForecastDto_ShouldSupportRoundedValues(double original, double expected)
        {
            // Arrange & Act
            var result = new ResultTimeSeriesForecastDto
            {
                Date = DateTime.Now,
                PredictedValue = Math.Round(original, 2)
            };

            // Assert
            Assert.Equal(expected, result.PredictedValue);
        }

        [Fact]
        public void ForecastDto_WithZeroDays_ShouldStoreValue()
        {
            // Arrange & Act
            var forecast = new BLL.DTOs.Filter.FilterTimeSeriesForecastDto
            {
                phenomenId = 1,
                placeId = 1,
                days = 0
            };

            // Assert
            Assert.Equal(0, forecast.days);
        }

        [Fact]
        public void ForecastDto_WithNegativeDays_ShouldStoreValue()
        {
            // Arrange & Act
            var forecast = new BLL.DTOs.Filter.FilterTimeSeriesForecastDto
            {
                phenomenId = 1,
                placeId = 1,
                days = -1
            };

            // Assert
            Assert.Equal(-1, forecast.days);
            // Note: Validation should happen in handler, not DTO
        }

        [Fact]
        public void TimeSeriesForecastDto_ForecastsShouldBeOrdered()
        {
            // Arrange
            var dto = new TimeSeriesForecastDto
            {
                PhenomenId = 1,
                PlaceId = 1,
                LastDate = new DateTime(2024, 1, 1),
                Forecasts = new List<ResultTimeSeriesForecastDto>
                {
                    new ResultTimeSeriesForecastDto { Date = new DateTime(2024, 1, 2), PredictedValue = 10 },
                    new ResultTimeSeriesForecastDto { Date = new DateTime(2024, 1, 3), PredictedValue = 11 },
                    new ResultTimeSeriesForecastDto { Date = new DateTime(2024, 1, 4), PredictedValue = 12 }
                }
            };

            // Act & Assert
            for (int i = 0; i < dto.Forecasts.Count - 1; i++)
            {
                Assert.True(dto.Forecasts[i].Date < dto.Forecasts[i + 1].Date);
            }
        }

        [Fact]
        public void ResultTimeSeriesForecastDto_WithNegativePredictedValue_ShouldStoreValue()
        {
            // Arrange & Act
            var result = new ResultTimeSeriesForecastDto
            {
                Date = DateTime.Now,
                PredictedValue = -5.5
            };

            // Assert
            Assert.Equal(-5.5, result.PredictedValue);
        }

        [Fact]
        public void TimeSeriesForecastDto_LastDateShouldBeBeforeForecasts()
        {
            // Arrange
            var lastDate = new DateTime(2024, 1, 10);
            var dto = new TimeSeriesForecastDto
            {
                PhenomenId = 1,
                PlaceId = 1,
                LastDate = lastDate,
                Forecasts = new List<ResultTimeSeriesForecastDto>
                {
                    new ResultTimeSeriesForecastDto { Date = new DateTime(2024, 1, 11), PredictedValue = 10 },
                    new ResultTimeSeriesForecastDto { Date = new DateTime(2024, 1, 12), PredictedValue = 11 }
                }
            };

            // Assert
            Assert.All(dto.Forecasts, f => Assert.True(f.Date > dto.LastDate));
        }
    }
}