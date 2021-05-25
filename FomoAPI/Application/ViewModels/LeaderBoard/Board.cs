using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.ViewModels.LeaderBoard
{
    /// <summary>
    /// Leader board Listing the rankings for a query.
    /// </summary>
    public class Board
    {
        public string Header { get; private set; }

        public string ColumnHeaderName { get; private set; }

        public string ColumnHeaderValue { get; private set; }

        public IEnumerable<BoardValue> Values { get; private set; }

        [JsonConstructor]
        public Board(BoardColumns columns, IEnumerable<BoardValue> values)
        {
            Header = columns.Header;
            ColumnHeaderName = columns.ColumnHeaderName;
            ColumnHeaderValue = columns.ColumnHeaderValue;
            Values = values;
        }
    }
}
