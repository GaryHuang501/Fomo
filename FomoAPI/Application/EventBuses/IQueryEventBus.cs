﻿using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses
{
    /// <summary>
    /// Queues the requests to the stock client to prevent the request per interval threshold from being exceeded.
    /// Also saves and caches the results.
    /// It is intended to be used with a scheduler that will periodically call this class to execute the query
    /// and refresh the query counter.
    /// </summary>
    public interface IQueryEventBus
    {
        /// <summary>
        /// Execute the next set of pending queries.
        /// </summary>
        /// <remarks>
        /// If the number of executed queries exceeds the threshold for the interval, no queries
        /// will be run.
        /// </remarks>
        Task ExecutePendingQueries();

        /// <summary>
        /// Set the max queries that can be run per threshold.
        /// </summary>
        /// <param name="maxQueryPerInterval">Max Query Threshold</param>
        Task SetMaxQueryPerIntervalThreshold(int maxQueryPerInterval);

        /// <summary>
        /// Resets to a starting state
        /// </summary>
        Task Reset();

        /// <summary>
        /// Gets the number of queries ran last interval.
        /// </summary>
        int QueriesExecutedInterval { get; }
    }
}
