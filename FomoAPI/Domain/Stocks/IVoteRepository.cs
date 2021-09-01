using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks
{
    /// <summary>
    /// CRUD operations for stock symbol voting.
    /// </summary>
    public interface IVoteRepository
    {
        /// <summary>
        /// Save the vote for a given symbol and user.
        /// </summary>
        /// <param name="vote">The vote to save.</param>
        /// <returns>True if vote successfully added.</returns>
        Task<bool> SaveVote(Vote vote);

        /// <summary>
        /// Gets a dictionary of <see cref="TotalVotes"/> keyed by the SymbolID.
        /// </summary>
        /// <param name="symbolIds">SymbolIds to get total votes for.</param>
        /// <param name="userId">Id of the user displaying their portfolio.</param>
        /// <returns>The <see cref="IReadOnlyCollection{T}"/> of <see cref="TotalVotes"/> keyed by SymbolId and votes for the current user.</returns>
        Task<IReadOnlyDictionary<int, TotalVotes>> GetTotalVotes(ISet<int> symbolIds, Guid userId);
    }
}
