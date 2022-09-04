using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Exchanges;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Repositories
{
    public interface IExchangeSyncRepository
    {
        Task<int> AddSymbols(IEnumerable<InsertSymbolAction> symbols, int? batchSize = null);

        Task<int> DelistSymbols(IEnumerable<int> symbolIds);

        Task<int> RelistSymbols(IEnumerable<int> symbolIds);

        Task<int> UpdateSymbols(IEnumerable<UpdateSymbolAction> symbols);

        Task AddSyncHistory(string actionName, int symbolsChanged, string message, string error = null);

        Task<ExchangeSyncSetting> GetSyncSettings();

        Task<IReadOnlyDictionary<SymbolKey, Symbol>> GetExistingSymbols();
    }
}
