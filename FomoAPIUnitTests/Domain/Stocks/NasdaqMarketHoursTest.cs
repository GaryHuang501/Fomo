using FomoAPI.Domain.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIUnitTests.Domain.Stocks
{
    public class NasdaqMarketHoursTest
    {
        [Theory]
        [InlineData(13, 30, true)]
        [InlineData(18, 30, true)]
        [InlineData(20, 0, true)]
        [InlineData(13, 29, false)]
        [InlineData(12, 59, false)]
        [InlineData(20, 1, false)]
        [InlineData(10, 0, false)]
        [InlineData(23, 0, false)]
        public void IsMarketHours_ShouldReturnTrue_WhenWithinUTCHours(int hours, int minutes, bool expectedIsWithinHours)
        {
            var marketHours = new NasdaqMarketHours();

            var testDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, hours, minutes, 0);
            Assert.Equal(expectedIsWithinHours, marketHours.IsMarketHours(testDate));
        }
    }
}
