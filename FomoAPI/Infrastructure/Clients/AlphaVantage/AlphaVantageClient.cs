﻿using FomoAPI.Application.ConfigurationOptions;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.WebUtilities;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Clients.AlphaVantage.Parsers;
using Microsoft.Extensions.Logging;
using System.Web.Http;
using System.Collections.Generic;
using System.Linq;
using FomoAPI.Application.Services;

namespace FomoAPI.Infrastructure.Clients.AlphaVantage
{
    /// <inheritdoc cref="IStockClient"/>
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

            var content = await response.Content.ReadAsAsync<AlphaVantageSymbolSearchResponse>();

            _logger.LogTrace("Executing query http request url: {url}", urlWithQueryString);

            return content.BestMatches.Select(m => new SymbolSearchResult(symbol: m.Symbol, fullName: m.Name, m.MatchScore));
        }

        public async Task<AlphaVantageQueryResult<StockSingleQuoteData>> GetSingleQuoteData(AlphaVantageSingleQuoteQuery query)
        {
            return await GetQueryData<StockSingleQuoteData>(query, _parserFactory.GetSingleQuoteDataParser());
        }

        /// <summary>
        /// Run the given query against Alpha Vantage client.
        /// Adds the necessary headers such api key and from the query object
        /// </summary>
        /// <param name="query">Query object used to generate the request</param>
        /// <returns></returns>
        private async Task<AlphaVantageQueryResult<T>> GetQueryData<T>(IAlphaVantageQuery query, IAlphaVantageDataParser<T> parser) where T: IQueryableData
        {
            var parameters = query.GetParameters();
            AlphaVantageQueryResult<T> queryResult;

            try
            {
                string urlWithQueryString = QueryHelpers.AddQueryString(_alphaVantageOptions.Url, query.GetParameters());
                urlWithQueryString = QueryHelpers.AddQueryString(urlWithQueryString, AlphaVantageQueryKeys.ApiKey, _alphaVantageOptions.ApiKey);

                var client = _clientFactory.CreateClient(_alphaVantageOptions.ClientName);

                _logger.LogTrace("Executing query http request url: {url}", urlWithQueryString);

                var response = await client.GetAsync(urlWithQueryString);

                if (response.IsSuccessStatusCode)
                {
                    T data = default;

                    using (Stream stream = await response.Content.ReadAsStreamAsync())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        try
                        {
                            data = parser.ParseData(reader);
                            queryResult = new AlphaVantageQueryResult<T>(data);
                        }
                        catch(Exception)
                        {
                            var errorJson = await response.Content.ReadAsStringAsync();
                            var errorObject = JsonConvert.DeserializeObject<AlphaVantageQueryError>(errorJson);
                            queryResult = new AlphaVantageQueryResult<T>(error: errorObject.ErrorMessage);
                        }
                    }
                }
                else
                {
                    _logger.LogError("Failed to retrieve data from AlphaVantage: {reason}", response.ReasonPhrase);
                    queryResult = new AlphaVantageQueryResult<T>(error: response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Executing query http request");
                queryResult = new AlphaVantageQueryResult<T>(error: ex.Message);
            }

            return queryResult;
        }
    }
}
