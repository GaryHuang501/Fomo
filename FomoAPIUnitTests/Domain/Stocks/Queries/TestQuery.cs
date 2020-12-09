using FomoAPI.Application.EventBuses;
using FomoAPI.Application.EventBuses.QueryContexts;
using FomoAPI.Domain.Stocks.Queries;
using FomoAPI.Infrastructure.Enums;

namespace FomoAPIUnitTests.Domain.Stocks.Queries
{
    public class TestQuery : StockQuery
    {
        public TestQuery(int symbolId, QueryFunctionType functionType) : base(symbolId, functionType)
        {
        }

        public override IQueryContext CreateContext(IQueryContextFactory contextFactory)
        {
            throw new System.NotImplementedException();
        }
    }
}
