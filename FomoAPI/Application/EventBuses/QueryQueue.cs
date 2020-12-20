﻿using FomoAPI.Application.Stores;
using FomoAPI.Domain.Stocks.Queries;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FomoAPI.Application.EventBuses
{
    /// <summary>
    /// Thread-safe Queue for the next queries to run.
    /// </summary>
    /// <remarks>
    /// Queue will not enqueue queries that have been on the queue before until it has been cleared.
    /// </remarks>
    public class QueryQueue
    {
        private class StatusInfo
        {
            public QueryStatus Status { get; private set; }

            public DateTime DateCreated { get; private set; }

            public StatusInfo(QueryStatus status)
            {
                Status = status;
                DateCreated = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// The queue of queries to keep track of the next pneding queries to execute
        /// </summary>
        private readonly Queue<StockQuery> _queue;

        /// <summary>
        /// Hashset to keep track what queries are pending queue. 
        /// </summary>
        private readonly Dictionary<StockQuery, StatusInfo> _queryStatus;

        private readonly Dictionary<int, int> _numQueriesExecutedAtMinute;

        private readonly object _lock;

        public QueryQueue()
        {
            _queue = new Queue<StockQuery>();
            _queryStatus = new Dictionary<StockQuery, StatusInfo>();
            _numQueriesExecutedAtMinute = new Dictionary<int, int>();
            _lock = new object();
        }

        public bool IsEmpty()
        {
            lock(_lock)
            {
                return !_queue.Any();
            }
        }

        public int Count
        {
            get
            {
                lock(_lock)
                {
                    return _queue.Count;
                }
            }
        }


        /// <summary>
        /// Dequeue <see cref="StockQuery"/> items from queue equal to the given count.
        /// Dequeued queries are marked as executing.
        /// If count exceeds the set's size, the set will return all items
        /// </summary>
        /// <returns>The dequeued <see cref="Stockquery"/></returns>
        /// <remarks>IList is used to avoid deferred natured of IEnumreable potential causing race conditions.</remarks>
        public IList<StockQuery> Dequeue(int count)
        {
            lock (_lock)
            {
                int numItemsToDequeue = Math.Min(_queue.Count, count);
                var items = new List<StockQuery>();

                for(int i = 0; i < numItemsToDequeue; i++)
                {
                    StockQuery query = _queue.Dequeue();
                    _queryStatus[query] = new StatusInfo(QueryStatus.Executing);
                    items.Add(query);
                }

                return items;
            }
        }

        /// <summary>
        /// Get the number of queries ran for the current minute interval.
        /// </summary>
        /// <returns><see cref="int"/> Count of queries that have been executed and are executing at the current minute interval.</returns>
        public int GetCurrentIntervalQueriesRanCount()
        {
            lock(_lock){

                int currentMinute = DateTime.UtcNow.Minute;
                int numExecuted = 0;

                if (_numQueriesExecutedAtMinute.ContainsKey(currentMinute))
                {
                    numExecuted = _numQueriesExecutedAtMinute[currentMinute];
                }

                return numExecuted + _queryStatus.Count(q => q.Value.Status == QueryStatus.Executing);
            }
        }

        /// <summary>
        /// Query status is marked as Executed.
        /// </summary>
        /// <param name="query">Query to mark as executed.</param>
        public void MarkAsExecuted(StockQuery query)
        {
            lock (_lock)
            {
                if (_queryStatus.ContainsKey(query))
                {
                    _queryStatus[query] = new StatusInfo(QueryStatus.Executed);

                    int currentMinute = DateTime.UtcNow.Minute;

                    bool minuteExists = _numQueriesExecutedAtMinute.ContainsKey(currentMinute);

                    if (!minuteExists)
                    {
                        _numQueriesExecutedAtMinute.Add(currentMinute, 1);
                    }
                    else
                    {
                        _numQueriesExecutedAtMinute[currentMinute]++;
                    }
                }
            }
        }

        /// <summary>
        /// Clears all queries and sets queue to initial state.
        /// </summary>
        public void ClearAll()
        {
            lock (_lock)
            {
                _numQueriesExecutedAtMinute.Clear();
                _queryStatus.Clear();
                _queue.Clear();
            }
        }

        /// <summary>
        /// Add the query to the queue.
        /// Queries are marked as pending status.
        /// </summary>
        /// <param name="query">Query to add</param>
        /// <returns>True if query was added. False if query already exists and is not added</returns>
        /// <remarks>Query cannot be enqueued again until it has been <see cref="Clear(StockQuery)"/> has been called.</remarks>
        public bool Enqueue(StockQuery query)
        {
            lock (_lock)
            {
                bool queryExists = _queryStatus.TryGetValue(query, out StatusInfo statusInfo);

                if (!queryExists)
                {
                    _queue.Enqueue(query);
                    _queryStatus.Add(query, new StatusInfo(QueryStatus.Pending));

                    return true;
                }

                return false;
            }
        }
    }
}
