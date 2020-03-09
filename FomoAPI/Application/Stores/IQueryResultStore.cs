using FomoAPI.Application.EventBuses;

namespace FomoAPI.Application.Stores
{
    /// <summary>
    ///  Used to store the results of the query from event bus.
    /// </summary>
    public interface IQueryResultStore
    {
        void AddQuery(ISubscribableQuery query, ISubscriptionQueryResult result);

        ISubscriptionQueryResult GetQueryResult(ISubscribableQuery query);
    }
}
