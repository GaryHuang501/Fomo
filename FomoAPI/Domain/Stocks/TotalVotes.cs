using Dapper;
using Newtonsoft.Json;
using System;

namespace FomoAPI.Domain.Stocks
{
    public record TotalVotes
    {
        public Guid UserId { get; init; }

        public int SymbolId { get; init; }

        /// <summary>
        /// Gets the vote count.
        /// </summary>
        public int Count { get; init; }

        /// <summary>
        /// Gets the vote direction for the current user id.
        /// </summary>
        public VoteDirection MyVoteDirection { get; init; }
   
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
