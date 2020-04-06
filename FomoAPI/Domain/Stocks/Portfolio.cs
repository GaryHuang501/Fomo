using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks
{
    /// <summary>
    /// User created portfolio containing the stock symbols they added.
    /// </summary>
    public class Portfolio
    {
        public int Id { get; set; }

        public int Name { get; set; }

        public DateTime LastModified { get; set; }

        public DateTime DateCreated { get; set; }

        public IEnumerable<Symbol> Symbols { get; set; }
    }
}
