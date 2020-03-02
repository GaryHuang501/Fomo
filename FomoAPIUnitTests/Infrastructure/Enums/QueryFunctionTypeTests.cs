using FomoAPI.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace FomoAPIUnitTests.Infrastructure.Enums
{
    public class QueryFunctionTypeTests
    {
        public class ExpectedValues
        {
            public string FunctionName;

            public TimeSpan RenewalTime;
        }

        public static IEnumerable<object[]> FunctionTypeToExpectedValues => new List<object[]>
        {
               new object[] { QueryFunctionType.SingleQuote, new ExpectedValues { FunctionName = "GLOBAL_QUOTE", RenewalTime = TimeSpan.FromMinutes(5) } },
               new object[] { QueryFunctionType.IntraDay, new ExpectedValues { FunctionName = "TIME_SERIES_INTRADAY", RenewalTime = TimeSpan.FromMinutes(5) } },
               new object[] { QueryFunctionType.Daily, new ExpectedValues { FunctionName = "TIME_SERIES_DAILY", RenewalTime = TimeSpan.FromMinutes(15) } },
               new object[] { QueryFunctionType.Weekly, new ExpectedValues { FunctionName = "TIME_SERIES_WEEKLY", RenewalTime = TimeSpan.FromMinutes(30) } },
               new object[] { QueryFunctionType.Monthly, new ExpectedValues { FunctionName = "TIME_SERIES_MONTHLY", RenewalTime = TimeSpan.FromMinutes(30) } }
        };

        public static IEnumerable<object[]> FunctionTypes => FunctionTypeToExpectedValues.Select(x => new object[] { x.First() });

        [Theory]
        [MemberData(nameof(FunctionTypeToExpectedValues))]
        public void QueryFunctionName_ShouldMatchExpectedForEachFunctionType(QueryFunctionType functionType, ExpectedValues expectedValues)
        {
            Assert.Equal(expectedValues.FunctionName, functionType.QueryFunctionName);
        }

        [Theory]
        [MemberData(nameof(FunctionTypeToExpectedValues))]
        public void GetHashCode_ShouldReturnHashCodeOfFunctionName(QueryFunctionType functionType, ExpectedValues expectedValues)
        {

            Assert.Equal(expectedValues.FunctionName.GetHashCode(), functionType.QueryFunctionName.GetHashCode());
        }

        [Theory]
        [MemberData(nameof(FunctionTypeToExpectedValues))]
        public void RenewalTime_ShouldMatchExpectedForEachFunctionType(QueryFunctionType functionType, ExpectedValues expectedValues)
        {
            Assert.Equal(expectedValues.RenewalTime, functionType.RenewalTime);
        }

        [Theory]
        [MemberData(nameof(FunctionTypes))]
        public void IsExpired_ShouldReturnTrue_WhenMinutesElapsedExceedsRenewalTime(QueryFunctionType functionType)
        {
            var oldDate = DateTime.Parse("2019-01-01 11:00");
            var newDate = DateTime.Parse("2019-01-01 12:00");

            Assert.True(functionType.IsExpired(oldDate, newDate));
        }

        [Theory]
        [MemberData(nameof(FunctionTypes))]
        public void IsExpired_ShouldReturnFalse_WhenMinutesElapsedEqualRenewalTime(QueryFunctionType functionType)
        {
            var oldDate = DateTime.Parse("2019-01-01 11:00");
            var newDate = DateTime.Parse("2019-01-01 11:00");

            Assert.False(functionType.IsExpired(oldDate, newDate));
        }

        [Theory]
        [MemberData(nameof(FunctionTypes))]
        public void IsExpired_ShouldReturnFalse_WhenMinutesElapsedLessThanRenewalTime(QueryFunctionType functionType)
        {
            var oldDate = DateTime.Parse("2019-01-01 11:00");
            var newDate = DateTime.Parse("2019-01-01 10:00");

            Assert.False(functionType.IsExpired(oldDate, newDate));
        }

        [Fact]
        public void Equals_ShouldReturnTrue_WhenQueryFunctionTypeAndSameFunctionName()
        {
            Assert.Equal(QueryFunctionType.Daily, QueryFunctionType.Daily);
            Assert.Equal(QueryFunctionType.IntraDay, QueryFunctionType.IntraDay);
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenQueryFunctionTypeNotSame()
        {
            Assert.NotEqual(QueryFunctionType.Daily, QueryFunctionType.IntraDay);
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenNotQueryFunctionTypeClass()
        {
            Assert.NotEqual(QueryFunctionType.Daily, new object());
        }
    }
}
