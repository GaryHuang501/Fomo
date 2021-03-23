using FomoAPI.Domain.Stocks;
using Newtonsoft.Json;

namespace FomoAPI.Application.Commands.Vote
{
    /// <summary>
    /// Command to create or update a vote for a stock symbol.
    /// </summary>
    public class VoteCommand
    {
        public int SymbolId { get; set; }

        public VoteDirection Direction { get; set; }
    }
}
