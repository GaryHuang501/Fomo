using FomoAPI.Application.EventBuses.Triggers;
using Microsoft.AspNetCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    /// <summary>
    /// Used to calculate a threshold for a symbol change set 
    /// </summary>
    public class ThresholdCheck
    {
        public string ActionName { get; }

        public int ChangeCount { get; }

        public int ThresholdPercent { get; }

        public ThresholdCheck(string actionName, int changeCount, int thresholdPercent)
        {
            ActionName = actionName;
            ChangeCount = changeCount;
            ThresholdPercent = thresholdPercent;
        }

        public bool CheckThreshold(int originalAmount, out string error)
        {
            error = null;

            if(originalAmount <= 0)
            {
                error = "Cannot apply threshold when no symbols exists";
                return false;
            }

            double changedPercent = (double) ChangeCount / originalAmount * 100;

            if (changedPercent > ThresholdPercent)
            {
                error = $"Threshold exceeded for {ActionName}. Changed Percent was {changedPercent}";
                return false;
            }

            return true;
        }
    }
}
