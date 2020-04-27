using FomoAPI.Domain.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Repositories
{
    public interface ISymbolRepository
    {
        Task<IEnumerable<Symbol>> GetSymbols(string keyword);
    }
}
