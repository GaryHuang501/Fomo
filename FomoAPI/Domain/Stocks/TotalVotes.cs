using Dapper;
using Newtonsoft.Json;
using System;

namespace FomoAPI.Domain.Stocks
{
    public record TotalVotes
    {
        public Guid UserId { get; private set; }

        public int SymbolId { get; private set; }

        /// <summary>
        /// Gets the vote count.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the vote direction for the current user id.
        /// </summary>
        public VoteDirection MyVoteDirection { get; private set; }
   
        [ExplicitConstructor]
        [JsonConstructor]
        public TotalVotes(Guid userId, int symbolId, int count, VoteDirection myVoteDirection)
        {
            UserId = userId;
            SymbolId = symbolId;
            Count = count;
            MyVoteDirection = myVoteDirection;
        }
    }
}
