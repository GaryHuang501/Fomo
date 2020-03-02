using FomoAPI.Application.EventBuses;

namespace FomoAPI.Application.Caches
{
    public interface IQueryResultStore
    {
        void AddQuery(ISubscribableQuery query, ISubscriptionQueryResult result);

        ISubscriptionQueryResult GetQueryResult(ISubscribableQuery query);

        void RemoveQuery();
    }
}
