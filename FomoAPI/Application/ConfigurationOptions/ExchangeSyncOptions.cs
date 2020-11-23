using System;
using System.ComponentModel.DataAnnotations;

namespace FomoAPI.Application.ConfigurationOptions
{
    public class ExchangeSyncOptions
    {
        [Required]
        public int SyncIntervalMinutes { get; set; }

        public bool SyncOnStart { get; set; }
    }
}
