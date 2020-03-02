using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses
{
    public interface IQueryEventBus
    {
        void EnqueueNextQueries();

        Task ExecutePendingQueriesAsync();

        void SetMaxQueryPerMinuteThreshold(int maxQueryPerMinuteThreshold);

        void ResetQueryExecutedCounter();
    }
}
