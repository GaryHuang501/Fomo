using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks
{
    /// <summary>
    /// User's vote for a stock symbol.
    /// </summary>
    public record Vote
    {
        public Guid UserId { get; init; }

        public int SymbolId { get; init; }

        public VoteDirection Direction { get; init; }

        public DateTime LastUpdated { get; init; }

        public Vote(Guid userId, int symbolId, VoteDirection direction, DateTime lastUpdated)
        {
            UserId = userId;
            SymbolId = symbolId;
            Direction = direction;
            LastUpdated = lastUpdated;
        }
    }
}
