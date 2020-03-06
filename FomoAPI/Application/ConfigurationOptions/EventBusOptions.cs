using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.ConfigurationOptions
{
    public class EventBusOptions
    {
        /// <summary>
        /// How often the event bus should execute the queries
        /// </summary>
        public int DelayMSRunScheduleEventBus { get; set; }

        public int MaxQueriesPerInterval { get; set; }

    }
}
