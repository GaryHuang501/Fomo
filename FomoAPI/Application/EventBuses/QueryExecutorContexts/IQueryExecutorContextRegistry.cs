namespace FomoAPI.Application.EventBuses.QueryExecutorContexts
{
    public interface IQueryExecutorContextRegistry
    {
        IQueryExecutorContext<ISubscribableQuery, ISubscriptionQueryResult> GetExecutorContext(ISubscribableQuery query);
    }
}