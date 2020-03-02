using FomoAPI.Application.EventBuses.Triggers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses.QueryExecutorContexts
{
    public interface IQueryExecutorContext<TSubscribableQuery, TSubscriptionQueryResult>
        where TSubscribableQuery : ISubscribableQuery
        where TSubscriptionQueryResult : ISubscriptionQueryResult
    {

        Task<TSubscriptionQueryResult> FetchQueryResultAsync(TSubscribableQuery query);

        Task SaveToStoreAsync(TSubscribableQuery query, TSubscriptionQueryResult result);

        IEnumerable<IQueryResultTrigger> GetQueryResultTriggers();
    }
}
