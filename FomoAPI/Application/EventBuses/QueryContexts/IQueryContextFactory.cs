using FomoAPI.Domain.Stocks.Queries;

namespace FomoAPI.Application.EventBuses.QueryContexts
{
    public interface IQueryContextFactory
    {
        SingleQuoteContext GenerateSingleQuoteContext(SingleQuoteQuery query);
    }
}