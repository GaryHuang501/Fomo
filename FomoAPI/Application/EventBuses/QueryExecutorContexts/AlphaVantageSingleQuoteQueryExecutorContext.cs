using FomoAPI.Application.Stores;
using FomoAPI.Application.EventBuses.Triggers;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.AlphaVantage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses.QueryExecutorContexts
{
    public class AlphaVantageSingleQuoteQueryExecutorContext : IQueryExecutorContext<AlphaVantageSingleQuoteQuery, AlphaVantageQueryResult<StockSingleQuoteData>>
    {

        private readonly IQueryResultStore _queryResultCache;

        private readonly IAlphaVantageClient _alphaVantageClient;

        public AlphaVantageSingleQuoteQueryExecutorContext(
            IAlphaVantageClient alphaVantageClient,
            IQueryResultStore queryResultCache)
        {
            _alphaVantageClient = alphaVantageClient;
            _queryResultCache = queryResultCache;
        }

        public Task SaveToStoreAsync(AlphaVantageSingleQuoteQuery query, AlphaVantageQueryResult<StockSingleQuoteData> result)
        {
            _queryResultCache.AddQuery(query, result);
            return Task.CompletedTask;
        }

        public async Task<AlphaVantageQueryResult<StockSingleQuoteData>> FetchQueryResultAsync(AlphaVantageSingleQuoteQuery query)
        {
           return await _alphaVantageClient.GetSingleQuoteData(query);
        }

        public IEnumerable<IQueryResultTrigger> GetQueryResultTriggers()
        {
            return new List<IQueryResultTrigger> { new CheckSingleQuoteThresholdAlert() };
        }
    }
}

