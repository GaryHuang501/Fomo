using System;

namespace FomoAPI.Domain.Stocks
{
    /// <summary>
    /// Tracks the market hours for an exchange.
    /// </summary>
    public interface IMarketHours
    {
        int StartHoursUTC { get; }

        int StartMinutesUTC { get; }

        int EndHoursUTC { get; }

        int EndMinutesUTC { get; }

        bool IsMarketHours(DateTime date);
    }
}
