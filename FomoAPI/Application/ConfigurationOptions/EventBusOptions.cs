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
        /// Interval for How often the event bus should poll for new queries
        /// </summary>
        [Range(1, int.MaxValue)]
        public int PollingIntervalMS { get; set; }

        /// <summary>
        /// Interval for how often the event bus will refresh and reset the counter
        /// for how many queries left are allowed to execute.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int RefreshIntervalMS { get; set; }

        /// <summary>
        /// How many queries can be run for each delay specified by RefreshIntervalMS
        /// </summary>
        [Range(1, int.MaxValue)]
        public int MaxQueriesPerInterval { get; set; }

    }
}
