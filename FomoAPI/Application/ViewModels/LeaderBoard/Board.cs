using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.ViewModels.LeaderBoard
{
    public class Board
    {
        public string Column { get; private set; }

        public IEnumerable<BoardValue> Values { get; private set; }

        [JsonConstructor]
        public Board(string column, IEnumerable<BoardValue> values)
        {
            Column = column;
            Values = values;
        }
    }
}
