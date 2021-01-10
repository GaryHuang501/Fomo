using FomoAPI.Domain.Stocks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.DTOs
{
    /// <summary>
    /// DTO for <see cref="Domain.Stocks.SingleQuoteData"/> to client
    /// </summary>
    public class StockSingleQuoteDataDTO
    {
        public int SymbolId { get; private set; }

        /// <summary>
        /// Gets the <see cref="SingleQuoteData"/>.
        /// Null means no data exists.
        /// </summary>
        public SingleQuoteData Data { get; private set; }

        public DateTime? LastUpdated { get; private set; }

        /// <summary>
        /// Instantiate instance of <see cref="StockSingleQuoteDataDTO"/> with data.
        /// </summary>
        /// <param name="symbolId">SymbolId of Stock</param>
        /// <param name="singleQuoteData">The <see cref="SingleQuoteData"/></param>
        public StockSingleQuoteDataDTO(int symbolId, SingleQuoteData singleQuoteData)
        {
            SymbolId = symbolId > 0 ? symbolId : throw new ArgumentException(nameof(symbolId), "must be greater than 0");
            Data = singleQuoteData ?? throw new NullReferenceException(nameof(singleQuoteData));
            LastUpdated = singleQuoteData.LastUpdated;
        }

        /// <summary>
        /// Instantiate instance with no data or last updated date.
        /// </summary>
        /// <param name="symbolId">SymbolId of Stock</param>
        public StockSingleQuoteDataDTO(int symbolId)
        {
            SymbolId = symbolId > 0 ? symbolId : throw new ArgumentException(nameof(symbolId), "must be greater than 0");
            Data = null;
            LastUpdated = null;
        }

        [JsonConstructor]
        public StockSingleQuoteDataDTO(int symbolId, SingleQuoteData data, DateTime? lastUpdated)
        {
            SymbolId = symbolId;
            Data = data;
            LastUpdated = lastUpdated;
        }

        public static StockSingleQuoteDataDTO CreateNoDataExists(int symbolId)
        {
            return new StockSingleQuoteDataDTO(symbolId);
        }
    }
}
