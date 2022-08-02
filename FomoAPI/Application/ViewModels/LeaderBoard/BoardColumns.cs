using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.ViewModels.LeaderBoard
{
    /// <summary>
    /// Columns for a leader board.
    /// </summary>
    public class BoardColumns
    {
        public static BoardColumns MostBullish = new ("Most Bullish Stocks", "Ticker", "Votes");

        public static BoardColumns MostBearish = new ("Most Bearish Stocks", "Ticker", "Votes");

        public static BoardColumns BestPerformer = new ("Best Performing Users", "User", "ROI %");

        public static BoardColumns WorstPerformer = new ("Worst Performing Users", "User", "ROI %");

        public string Header { get; private set; }

        public string ColumnHeaderName { get; private set; }

        public string ColumnHeaderValue { get; private set; }

        public BoardColumns(string header, string columnHeaderName, string columnHeaderValue)
        {
            Header = header;
            ColumnHeaderName = columnHeaderName;
            ColumnHeaderValue = columnHeaderValue;
        }
    }
}
