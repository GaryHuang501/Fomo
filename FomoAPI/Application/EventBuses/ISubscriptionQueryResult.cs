using System;

namespace FomoAPI.Application.EventBuses
{
    public interface ISubscriptionQueryResult
    {
        bool HasError { get; }

        string ErrorMessage { get; }

        DateTime CreateDateUtc { get; }
    }
}
