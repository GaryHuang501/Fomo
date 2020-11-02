using FomoAPI.Application.EventBuses;

namespace FomoAPI.Application.Stores
{
    public interface IQueryCache
    {
        void Add(ISubscribableQuery query, ISubscriptionQueryResult result);

        bool TryGet(ISubscribableQuery query, out ISubscriptionQueryResult result);

        void Remove(ISubscribableQuery query);
    }
}
