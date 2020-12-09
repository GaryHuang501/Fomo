using FomoAPI.Application.Stores;
using FomoAPI.Domain.Stocks.Queries;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FomoAPI.Application.EventBuses
{
    /// <summary>
    /// Thread-safe Priority Set Collection for the next queries to run on the event bus.
    /// Queries are taken in a FIFO manner.
    /// Collection does not allow duplicate queries
    /// </summary>
    public class QueryPrioritySet
    {
        /// <summary>
        /// The queue of queries to keep track of the next query to execute on the event bus
        /// </summary>
        private readonly List<StockQuery> _queryList;

        /// <summary>
        /// Hashset to keep track what queries are in the queue. 
        /// </summary>
        private readonly HashSet<StockQuery> _queryHashset;

        private readonly object _lock;

        public QueryPrioritySet()
        {
            _queryList = new List<StockQuery>();
            _queryHashset = new HashSet<StockQuery>();
            _lock = new object();
        }

        public bool IsEmpty => _queryList.Count == 0;

        public int Count => _queryList.Count;

        /// <summary>
        /// Remove the query on the set 
        /// </summary>
        /// <param name="query">query to remove</param>
        public bool Remove(StockQuery query)
        {
            bool isSuccess;

            lock (_lock)
            {
                isSuccess = _queryList.Remove(query);

                if (isSuccess)
                {
                    _queryHashset.Remove(query);
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// Take the range of queries from the beginning of the set.
        /// If count exceeds the set's size, the set will return all items
        /// </summary>
        /// <param name="count">Number of queries to remove</param>
        /// <returns>The queries</returns>
        public IEnumerable<StockQuery> Take(int count)
        {
            lock (_lock)
            {
                return _queryList.Take(Math.Min(_queryList.Count, count));
            }
        }

        /// <summary>
        /// Add the query to the end of the list
        /// </summary>
        /// <param name="query">Query to add</param>
        /// <returns>true if query was added. False if query already exists and is not added</returns>
        public bool TryAdd(StockQuery query)
        {
            lock (_lock)
            {
                if (_queryHashset.Add(query))
                {
                    _queryList.Add(query);
                    return true;
                }
            }

            return false;
        }
    }
}
