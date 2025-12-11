using AirAnalysis.BLL.DTOs.Filter;
using AirAnalysis.BLL.Queries.Anomaly;
using Xunit;

namespace AirAnalysis.UnitTests.Queries.Anomaly
{
    public class GetAnomalyHandlerTests
    {
        [Fact]
        public void GetAnomalyQuery_ShouldAcceptFilterAnomalyDto()
        {
            // Arrange
            var filter = new FilterAnomalyDto
            {
                PhenomenIds = 1,
                PlaceIds = 2,
                DataStart = new DateTime(2024, 1, 1),
                DataEnd = new DateTime(2024, 1, 31)
            };

            // Act
            var query = new GetAnomalyQuery(filter);

            // Assert
            Assert.NotNull(query);
            Assert.Equal(filter, query.forecast);
        }

        [Fact]
        public void FilterAnomalyDto_ShouldStoreCorrectValues()
        {
            // Arrange & Act
            var filter = new FilterAnomalyDto
            {
                PhenomenIds = 5,
                PlaceIds = 10,
                DataStart = new DateTime(2024, 3, 15),
                DataEnd = new DateTime(2024, 6, 20)
            };

            // Assert
            Assert.Equal(5, filter.PhenomenIds);
            Assert.Equal(10, filter.PlaceIds);
            Assert.Equal(new DateTime(2024, 3, 15), filter.DataStart);
            Assert.Equal(new DateTime(2024, 6, 20), filter.DataEnd);
        }

        [Fact]
        public void FilterAnomalyDto_WithDifferentPhenomenIds_ShouldBeDistinct()
        {
            // Arrange
            var filter1 = new FilterAnomalyDto
            {
                PhenomenIds = 1,
                PlaceIds = 1,
                DataStart = DateTime.Now,
                DataEnd = DateTime.Now
            };
            var filter2 = new FilterAnomalyDto
            {
                PhenomenIds = 2,
                PlaceIds = 1,
                DataStart = DateTime.Now,
                DataEnd = DateTime.Now
            };

            // Assert
            Assert.NotEqual(filter1.PhenomenIds, filter2.PhenomenIds);
        }

        [Fact]
        public void FilterAnomalyDto_WithDifferentPlaceIds_ShouldBeDistinct()
        {
            // Arrange
            var filter1 = new FilterAnomalyDto
            {
                PhenomenIds = 1,
                PlaceIds = 1,
                DataStart = DateTime.Now,
                DataEnd = DateTime.Now
            };
            var filter2 = new FilterAnomalyDto
            {
                PhenomenIds = 1,
                PlaceIds = 2,
                DataStart = DateTime.Now,
                DataEnd = DateTime.Now
            };

            // Assert
            Assert.NotEqual(filter1.PlaceIds, filter2.PlaceIds);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 5)]
        [InlineData(100, 200)]
        public void FilterAnomalyDto_ShouldAcceptDifferentIds(int phenomenId, int placeId)
        {
            // Arrange & Act
            var filter = new FilterAnomalyDto
            {
                PhenomenIds = phenomenId,
                PlaceIds = placeId,
                DataStart = new DateTime(2024, 1, 1),
                DataEnd = new DateTime(2024, 12, 31)
            };

            // Assert
            Assert.Equal(phenomenId, filter.PhenomenIds);
            Assert.Equal(placeId, filter.PlaceIds);
        }

        [Fact]
        public void FilterAnomalyDto_WithSameStartAndEndDate_ShouldBeValid()
        {
            // Arrange
            var sameDate = new DateTime(2024, 6, 15);

            // Act
            var filter = new FilterAnomalyDto
            {
                PhenomenIds = 1,
                PlaceIds = 1,
                DataStart = sameDate,
                DataEnd = sameDate
            };

            // Assert
            Assert.Equal(sameDate, filter.DataStart);
            Assert.Equal(sameDate, filter.DataEnd);
            Assert.Equal(filter.DataStart, filter.DataEnd);
        }

        [Fact]
        public void FilterAnomalyDto_WithStartBeforeEnd_ShouldStoreCorrectly()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 12, 31);

            // Act
            var filter = new FilterAnomalyDto
            {
                PhenomenIds = 1,
                PlaceIds = 1,
                DataStart = startDate,
                DataEnd = endDate
            };

            // Assert
            Assert.True(filter.DataStart < filter.DataEnd);
            Assert.Equal(startDate, filter.DataStart);
            Assert.Equal(endDate, filter.DataEnd);
        }

        [Fact]
        public void GetAnomalyQuery_WithMultipleCreations_ShouldBeIndependent()
        {
            // Arrange
            var filter1 = new FilterAnomalyDto
            {
                PhenomenIds = 1,
                PlaceIds = 1,
                DataStart = new DateTime(2024, 1, 1),
                DataEnd = new DateTime(2024, 1, 31)
            };

            var filter2 = new FilterAnomalyDto
            {
                PhenomenIds = 2,
                PlaceIds = 2,
                DataStart = new DateTime(2024, 2, 1),
                DataEnd = new DateTime(2024, 2, 28)
            };

            // Act
            var query1 = new GetAnomalyQuery(filter1);
            var query2 = new GetAnomalyQuery(filter2);

            // Assert
            Assert.NotEqual(query1.forecast.PhenomenIds, query2.forecast.PhenomenIds);
            Assert.NotEqual(query1.forecast.PlaceIds, query2.forecast.PlaceIds);
            Assert.NotEqual(query1.forecast.DataStart, query2.forecast.DataStart);
        }

        [Theory]
        [InlineData(2024, 1, 1, 2024, 1, 31)]
        [InlineData(2024, 2, 1, 2024, 2, 29)] // Leap year
        [InlineData(2024, 12, 1, 2024, 12, 31)]
        [InlineData(2024, 6, 15, 2024, 6, 15)] // Same date
        public void FilterAnomalyDto_WithVariousDateRanges_ShouldAcceptAll(
            int startYear, int startMonth, int startDay,
            int endYear, int endMonth, int endDay)
        {
            // Arrange & Act
            var filter = new FilterAnomalyDto
            {
                PhenomenIds = 1,
                PlaceIds = 1,
                DataStart = new DateTime(startYear, startMonth, startDay),
                DataEnd = new DateTime(endYear, endMonth, endDay)
            };

            // Assert
            Assert.NotNull(filter);
            Assert.True(filter.DataStart <= filter.DataEnd);
        }

        [Fact]
        public void FilterAnomalyDto_IsRecordType_ShouldSupportValueEquality()
        {
            // Arrange
            var filter1 = new FilterAnomalyDto
            {
                PhenomenIds = 1,
                PlaceIds = 2,
                DataStart = new DateTime(2024, 1, 1),
                DataEnd = new DateTime(2024, 12, 31)
            };

            var filter2 = new FilterAnomalyDto
            {
                PhenomenIds = 1,
                PlaceIds = 2,
                DataStart = new DateTime(2024, 1, 1),
                DataEnd = new DateTime(2024, 12, 31)
            };

            // Assert - record types support value-based equality
            Assert.Equal(filter1, filter2);
        }

        [Fact]
        public void FilterAnomalyDto_WithDifferentValues_ShouldNotBeEqual()
        {
            // Arrange
            var filter1 = new FilterAnomalyDto
            {
                PhenomenIds = 1,
                PlaceIds = 2,
                DataStart = new DateTime(2024, 1, 1),
                DataEnd = new DateTime(2024, 12, 31)
            };

            var filter2 = new FilterAnomalyDto
            {
                PhenomenIds = 1,
                PlaceIds = 3, // Different PlaceId
                DataStart = new DateTime(2024, 1, 1),
                DataEnd = new DateTime(2024, 12, 31)
            };

            // Assert
            Assert.NotEqual(filter1, filter2);
        }

        [Fact]
        public void GetAnomalyQuery_ShouldStoreFilterReference()
        {
            // Arrange
            var filter = new FilterAnomalyDto
            {
                PhenomenIds = 5,
                PlaceIds = 10,
                DataStart = new DateTime(2024, 3, 15),
                DataEnd = new DateTime(2024, 6, 20)
            };

            // Act
            var query = new GetAnomalyQuery(filter);

            // Assert
            Assert.Same(filter, query.forecast);
        }

        [Fact]
        public void FilterAnomalyDto_DateRange_ShouldCalculateCorrectly()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var filter = new FilterAnomalyDto
            {
                PhenomenIds = 1,
                PlaceIds = 1,
                DataStart = startDate,
                DataEnd = endDate
            };

            // Act
            var daysDifference = (filter.DataEnd - filter.DataStart).Days;

            // Assert
            Assert.Equal(30, daysDifference);
        }

        [Fact]
        public void FilterAnomalyDto_WithMinMaxDates_ShouldWork()
        {
            // Arrange & Act
            var filter = new FilterAnomalyDto
            {
                PhenomenIds = 1,
                PlaceIds = 1,
                DataStart = DateTime.MinValue,
                DataEnd = DateTime.MaxValue
            };

            // Assert
            Assert.Equal(DateTime.MinValue, filter.DataStart);
            Assert.Equal(DateTime.MaxValue, filter.DataEnd);
        }
    }
}