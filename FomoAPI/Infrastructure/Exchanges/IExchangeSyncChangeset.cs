using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    /// <summary>
    /// A Collection of a specific changes needed to apply to database symbol table
    /// to sync with exchanges.
    /// </summary>
    public interface IExchangeSyncChangeset
    {
        void AddChange(SymbolKey symbolKey);

        bool HasChanges(SymbolKey symbolKey);

        Task SaveChangeset(IExchangeSyncRepository repository);

        bool SafeThreshold(ExchangeSyncSetting setting, out string error);
    }
}
