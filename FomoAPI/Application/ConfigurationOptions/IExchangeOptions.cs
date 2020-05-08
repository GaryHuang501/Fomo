namespace FomoAPI.Application.ConfigurationOptions
{
    public interface IExchangeOptions
    {
        string Url { get; set; }

        string ClientName { get; set; }

        string Delimiter { get; set; }

        string[] SuffixBlackList { get; set; }
    }
}
