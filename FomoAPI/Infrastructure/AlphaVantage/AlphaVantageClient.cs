using FomoAPI.Application.ConfigurationOptions;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.WebUtilities;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.AlphaVantage.Parsers;
using Microsoft.Extensions.Logging;
using System.Web.Http;

namespace FomoAPI.Infrastructure.AlphaVantage
{
    /// <summary>
    /// HTTP Client wrapper class to fetch data from Alpha Vantage API
    /// </summary>
    public class AlphaVantageClient : IAlphaVantageClient
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

        /// <summary>
        /// Execute Query for Single Quote (Global Quote) Data
        /// </summary>
        /// <param name="query">Query Object for Single Quote Data</param>
        /// <returns></returns>
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
                var urlWithQueryString = QueryHelpers.AddQueryString(_alphaVantageOptions.Url, query.GetParameters());
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
