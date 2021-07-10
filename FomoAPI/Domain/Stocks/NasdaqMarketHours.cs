﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks
{
    /// <summary>
    /// Market hours for Nasdaq.
    /// </summary>
    /// <remarks>All major American Exchanges follow the same hours.</remarks>
    public class NasdaqMarketHours : IMarketHours
    {
        public int StartHoursUTC => 13;

        public int StartMinutesUTC => 30;

        public int EndHoursUTC => 20;

        public int EndMinutesUTC => 0;

        public bool IsMarketHours(DateTime date)
        {
            var timeOfDay = date.TimeOfDay;
            var startOfDay = new TimeSpan(StartHoursUTC, StartMinutesUTC, 0);
            var endOfDay = new TimeSpan(EndHoursUTC, EndMinutesUTC, 0);

            return timeOfDay >= startOfDay && timeOfDay <= endOfDay;
        }
    }
}
