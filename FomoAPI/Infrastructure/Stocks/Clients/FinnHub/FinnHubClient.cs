using FomoAPI.Domain.Stocks;
using FomoAPI.Domain.Stocks.Queries;
using FomoAPI.Infrastructure.ConfigurationOptions;
using FomoAPI.Infrastructure.Stocks;
using FomoAPI.Infrastructure.Stocks.Clients.FinnHub.Data;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Clients.FinnHub
{
    /// <summary>
    /// Client to send requests to the third party FinnHub API for stock information.
    /// </summary>
    public class FinnHubClient : IStockClient
    {
        private readonly ILogger _logger;

        private readonly FinnHubOptions _options;

        private readonly IHttpClientFactory _clientFactory;

        public FinnHubClient(IHttpClientFactory clientFactory, IOptionsMonitor<FinnHubOptions> optionsAccessor, ILogger<FinnHubClient> logger)
        {
            _clientFactory = clientFactory;
            _options = optionsAccessor.CurrentValue;
            _logger = logger;
        }

        public async Task<IEnumerable<SymbolSearchResult>> GetSearchedTickers(string keywords)
        {
            var queryValues = new Dictionary<string, string>
                {
                    { "q", keywords },
                    { "token", _options.ApiKey}
                };

            string url = QueryHelpers.AddQueryString($"{_options.Url}/search", queryValues);

            var client = _clientFactory.CreateClient(_options.ClientName);

            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var searchResponse = await response.Content.ReadAsAsync<FinnHubSymbolSearchResponse>();

            _logger.LogTrace("Executing query http request url: {url}", url);

            if (searchResponse.Result == null)
            {
                return new List<SymbolSearchResult>();
            }

            int rank = 1;

            return searchResponse.Result.Take(_options.SearchLimit)
                .Select(m => new SymbolSearchResult(ticker: m.Symbol, fullName: m.Description, rank++))
                .ToList();
        }

        public async Task<SingleQuoteQueryResult> GetSingleQuoteData(StockQuery query, string ticker, string exchangeName)
        {
            try
            {
                var queryValues = new Dictionary<string, string>
                {
                    { "symbol", ticker },
                    { "token", _options.ApiKey}
                };

                string url = QueryHelpers.AddQueryString($"{ _options.Url}/quote", queryValues);

                var client = _clientFactory.CreateClient(_options.ClientName);

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = await response.Content.ReadAsAsync<FinnHubSingleQuote>();

                        return new SingleQuoteQueryResult(ticker, result.ToDomain(query.SymbolId, ticker));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Failed to get single quote for {url} {error}", url, ex);
                        return new SingleQuoteQueryResult(ex.Message);
                    }
                }
                else
                {
                    _logger.LogError("Failed to retrieve single quote data for {url}: {reason}", url, response.ReasonPhrase);
                    return new SingleQuoteQueryResult(errorMessage: response.ReasonPhrase);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("Failed to get single quote for {ticker}", ticker);
                return new SingleQuoteQueryResult(ex.Message);
            }
        }
    }
}
