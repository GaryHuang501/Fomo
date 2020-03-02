using FomoAPI.Application.EventBuses;
using FomoAPI.Infrastructure.Enums;

namespace FomoAPIUnitTests.Application.EventBuses
{
    public class TestQuery : AbstractSubscribableQuery
    {
        public TestQuery(QueryFunctionType functionType, string symbol) : base(functionType, symbol)
        {
        }
    }
}
