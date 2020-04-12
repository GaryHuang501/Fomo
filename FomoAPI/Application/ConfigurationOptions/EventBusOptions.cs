using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.ConfigurationOptions
{
    public class EventBusOptions
    {
        /// <summary>
        /// How often the event bus should execute the queries
        /// </summary>
        [Range(1, int.MaxValue)]
        public int DelayMSRunScheduleEventBus { get; set; }

        /// <summary>
        /// How many queries can be run for each delay specified by DelayMSRunScheduleEventBus
        /// </summary>
        [Range(1, int.MaxValue)]
        public int MaxQueriesPerInterval { get; set; }

    }
}
