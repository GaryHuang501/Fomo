using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    /// <summary>
    /// Changes between symbols from exchange client and current database that need to be
    /// synced to our database.
    /// </summary>
    public interface IExchangeSyncChangeset
    {
        void AddChange(SymbolKey symbolKey);

        bool HasChanges(SymbolKey symbolKey);

        Task SaveChangeset(IExchangeSyncRepository repository);

        ThresholdCheck GetThresholdCheck(ExchangeSyncSetting setting);
    }
}
