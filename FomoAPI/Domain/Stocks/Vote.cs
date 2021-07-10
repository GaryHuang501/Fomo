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
        public Guid UserId { get; private set; }

        public int SymbolId { get; private set; }

        public VoteDirection Direction { get; private set; }

        public DateTime LastUpdated { get; private set; }

        public Vote(Guid userId, int symbolId, VoteDirection direction, DateTime lastUpdated)
        {
            UserId = userId;
            SymbolId = symbolId;
            Direction = direction;
            LastUpdated = lastUpdated;
        }
    }
}
