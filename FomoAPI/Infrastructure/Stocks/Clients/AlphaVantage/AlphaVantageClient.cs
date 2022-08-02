using FomoAPI.Application.ConfigurationOptions;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.WebUtilities;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Stocks.Clients.AlphaVantage.Parsers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using FomoAPI.Domain.Stocks.Queries;
using FomoAPI.Infrastructure.Enums;
using FomoAPI.Infrastructure.Stocks.Clients.AlphaVantage.Data;
using FomoAPI.Infrastructure.ConfigurationOptions;
using FomoAPI.Infrastructure.Stocks;

namespace FomoAPI.Infrastructure.Stocks.Clients.AlphaVantage
{
    /// Client to send requests to the third party AlphaVantage API for stock information.
    public class AlphaVantageClient : IStockClient
    {
        private readonly AlphaVantageOptions _alphaVantageOptions;

        private readonly IAlphaVantageDataParserFactory _parserFactory;

        private readonly ILogger _logger;

        private readonly IHttpClientFactory _clientFactory;

        public AlphaVantageClient(IHttpClientFactory clientFactory, IOptionsMonitor<AlphaVantageOptions> alphaVantageOptionsAccessor, IAlphaVantageDataParserFactory parserFactory, ILogger<AlphaVantageClient> logger)
        {
            _clientFactory = clientFactory;
            _alphaVantageOptions = alphaVantageOptionsAccessor.CurrentValue;
            _parserFactory = parserFactory;
            _logger = logger;
        }

        public async Task<IEnumerable<SymbolSearchResult>> GetSearchedTickers(string keywords)
        {
            //  ?function = SYMBOL_SEARCH & keywords = tesco & apikey = demo
            var queryValues = new Dictionary<string, string>
                {
                    {AlphaVantageQueryKeys.Function, "SYMBOL_SEARCH" },
                    {AlphaVantageQueryKeys.KeyWords, keywords},
                    {AlphaVantageQueryKeys.ApiKey, _alphaVantageOptions.ApiKey}
                };

            string urlWithQueryString = QueryHelpers.AddQueryString(_alphaVantageOptions.Url, queryValues);

            var client = _clientFactory.CreateClient(_alphaVantageOptions.ClientName);

            var response = await client.GetAsync(urlWithQueryString);         

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(); ;
            var content = await response.Content.ReadAsAsync<AlphaVantageSymbolSearchResponse>();

            _logger.LogTrace("Executing query http request url: {url}", urlWithQueryString);

            if(content.BestMatches == null)
            {
                return new List<SymbolSearchResult>();
            }

            int rank = 1;

            return content.BestMatches.Select(m => new SymbolSearchResult(ticker: m.Symbol, fullName: m.Name, rank++)).ToList();
        }

        public async Task<SingleQuoteQueryResult> GetSingleQuoteData(StockQuery query, string ticker, string exchangeName)
        {
            var alphaVantageQuery = new AlphaVantageQuery(QueryFunctionType.SingleQuote, ticker);

            var alphaVantageQueryResult = await GetQueryData<SingleQuoteData>(query.SymbolId, alphaVantageQuery, _parserFactory.GetSingleQuoteDataParser());

            if(alphaVantageQueryResult.HasError)
            {
                return new SingleQuoteQueryResult(alphaVantageQueryResult.ErrorMessage);
            }

            return new SingleQuoteQueryResult(ticker, alphaVantageQueryResult.Data);
        }

        /// <summary>
        /// Run the given query against Alpha Vantage client.
        /// Adds the necessary headers such api key and from the query object
        /// </summary>
        /// <param name="symbolId"></param>
        /// <param name="query">Query object used to generate the request</param>
        /// <param name="parser"><see cref="IAlphaVantageDataParser{TData}"/> to parse the response data.</param>
        /// <returns></returns>
        private async Task<AlphaVantageQueryResult<TData>> GetQueryData<TData>(int symbolId, AlphaVantageQuery query, IAlphaVantageDataParser<TData> parser)
            where TData : StockData  
        {
            AlphaVantageQueryResult<TData> queryResult;

            try
            {
                string urlWithQueryString = QueryHelpers.AddQueryString(_alphaVantageOptions.Url, query.GetParameters());
                urlWithQueryString = QueryHelpers.AddQueryString(urlWithQueryString, AlphaVantageQueryKeys.ApiKey, _alphaVantageOptions.ApiKey);

                var client = _clientFactory.CreateClient(_alphaVantageOptions.ClientName);

                _logger.LogTrace("Executing query http request url: {url}", urlWithQueryString);

                var response = await client.GetAsync(urlWithQueryString);

                TData data = null;

                if (response.IsSuccessStatusCode)
                {
                    string json = null;

                    try
                    {
                        json = await response.Content.ReadAsStringAsync();
                        data = parser.ParseJson(symbolId, json);
                        queryResult = new AlphaVantageQueryResult<TData>(data);
                    }
                    catch(Exception) when (json.Contains("Error Message"))
                    {
                        var errorJson = await response.Content.ReadAsStringAsync();
                        var errorObject = JsonConvert.DeserializeObject<AlphaVantageQueryError>(errorJson);
                        queryResult = new AlphaVantageQueryResult<TData>(error: errorObject);
                        _logger.LogError("Alphavantage Error for {url} query: {error}", urlWithQueryString, errorJson);
                    }
                    catch(Exception ex)
                    {
                        queryResult = new AlphaVantageQueryResult<TData>(errorMessage: $"Exception: {ex.Message}");
                        _logger.LogError(ex, "Failed to parse alphavantage data for query {url}", urlWithQueryString);
                    }
                }
                else
                {
                    _logger.LogError("Failed to retrieve single quote data from AlphaVantage: {reason}", response.ReasonPhrase);
                    queryResult = new AlphaVantageQueryResult<TData>(errorMessage: response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Executing query http request");
                queryResult = new AlphaVantageQueryResult<TData>(errorMessage: ex.Message);
            }

            return queryResult;
        }
    }
}
