namespace FomoAPI.Infrastructure.Exchanges
{
    public class ExchangeSyncSetting
    {
        public bool DisableSync { get; set; }

        public bool DisableThresholds { get; set; }

        public int InsertThresholdPercent { get; set; }

        public int DeleteThresholdPercent { get; set; }

        public int UpdateThresholdPercent { get; set; }

        public string Delimiter { get; set; }

        public string[] SuffixBlackList { get; set; }

        public string Url { get; set; }

        public string ClientName { get; set; }
    }
}
