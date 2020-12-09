using FomoAPI.Application.EventBuses;
using FomoAPI.Application.EventBuses.QueryContexts;
using FomoAPI.Domain.Stocks.Queries;
using FomoAPI.Infrastructure.Enums;
using Moq;

namespace FomoAPIUnitTests.Application.EventBuses
{
    public class TestQuery : StockQuery
    {
        public Mock<IQueryContext> _mockQueryContext;

        public TestQuery(int symbolId, QueryFunctionType functionType) : base(symbolId, functionType)
        {
        }

        public void SetMockQueryContext(Mock<IQueryContext> mockQueryContext)
        {
            _mockQueryContext = mockQueryContext;
        }

        public override IQueryContext CreateContext(IQueryContextFactory contextFactory)
        {
            return _mockQueryContext.Object;
        }
    }
}
