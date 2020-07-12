using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Exchanges;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Repositories
{
    public interface IExchangeSyncRepository
    {
        Task<int> AddSymbols(IEnumerable<Symbol> symbols, int? batchSize = null);

        Task<int> DelistSymbols(IEnumerable<int> symbolIds);

        Task<int> UpdateSymbols(IEnumerable<Symbol> symbols);

        Task AddSyncHistory(string actionName, int symbolsChanged, string message, string error = null);

        Task<ExchangeSyncSetting> GetSyncSettings();

        Task<IReadOnlyDictionary<SymbolKey, Symbol>> GetExistingSymbols();
    }
}
