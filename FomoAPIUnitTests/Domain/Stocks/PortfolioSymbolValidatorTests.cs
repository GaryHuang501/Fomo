using FomoAPI.Domain.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIUnitTests.Domain.Stocks
{
    public class PortfolioSymbolValidatorTests
    {
        [Theory]
        [InlineData(0, false)]
        [InlineData(0.1, true)]
        [InlineData(1, true)]
        [InlineData(10000, true)]
        public async Task Should_ValidateAvergaPriceGreaterThanZero(decimal averagePrice, bool expected)
        {
            var validator = new PortfolioSymbolValidator();

            var portfolioSymbol = new PortfolioSymbol(1, 1, "fake", "fake", "fake", averagePrice, false, 1);

            var result = await validator.ValidateAsync(portfolioSymbol);

            Assert.Equal(expected, result.IsValid);
        }
    }
}
