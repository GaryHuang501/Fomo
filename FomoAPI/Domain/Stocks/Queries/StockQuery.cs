using System;
using FomoAPI.Application.EventBuses.QueryContexts;
using FomoAPI.Common;
using FomoAPI.Infrastructure.Enums;

namespace FomoAPI.Domain.Stocks.Queries
{
    /// <summary>
    /// Base query class representing a users query to fetch a certain type of data for a stock.
    /// </summary>
    /// <remarks>This will be added to the queue to be be executed against the stock provider.</remarks>
    public abstract record StockQuery
    {
        public QueryFunctionType FunctionType { get; }

        public int SymbolId { get; }

        public DateTime CreateDate { get; }

        protected StockQuery(int symbolId, QueryFunctionType functionType)
        {
            FunctionType = functionType;
            SymbolId = symbolId;
            CreateDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates the context <see cref="IQueryContext"/> for the query.
        /// </summary>
        /// <param name="contextFactory">Factory to generate the context</param>
        /// <returns>The <see cref="IQueryContext"/></returns>
        /// <remarks>Uses the visitor pattern generating the context.</remarks>
        public abstract IQueryContext CreateContext(IQueryContextFactory contextFactory);

        public virtual bool Equals(StockQuery other)
        {
            var otherQuery = other as StockQuery;

            return otherQuery != null
                && otherQuery.SymbolId == SymbolId
                && otherQuery.FunctionType == FunctionType;
        }

        public override int GetHashCode()
        {
            return this.CalculateHashCodeForParams(SymbolId, FunctionType);
        }
    }
}
