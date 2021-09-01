using FomoAPI.Domain.Stocks;
using Newtonsoft.Json;

namespace FomoAPI.Application.Commands.Vote
{
    /// <summary>
    /// Command to vote on a stock symbol.
    /// </summary>
    public class VoteCommand
    {
        public int SymbolId { get; set; }

        public VoteDirection Direction { get; set; }
    }
}
