using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Enums
{
    /// <summary>
    /// Enums for Exchanges
    /// </summary>
    public class ExchangeType
    {
        private const string NasdaqName = "NASDAQ";
        private const string NyseName = "NYSE";
        private const string NyseArcaName = "NYSE ARCA";
        private const string NyseAmericanName = "NYSE AMERICAN";
        private const string UnknownName = "Unknown";

        public readonly static ExchangeType NASDAQ = new ExchangeType(1, NasdaqName);
        public readonly static ExchangeType NYSE = new ExchangeType(2, NyseName);
        public readonly static ExchangeType NYSEARCA = new ExchangeType(3, NyseArcaName);
        public readonly static ExchangeType NYSEAMERICAN = new ExchangeType(4, NyseAmericanName);
        public readonly static ExchangeType Unknown = new ExchangeType(-1, UnknownName);

        public string Name { get; private set; }
        public int Id { get; private set; }

        public ExchangeType(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static ExchangeType ToExchange(string name)
        {
            return name switch
            {
                NasdaqName => NASDAQ,
                NyseName => NYSE,
                NyseArcaName => NYSEARCA,
                NyseAmericanName => NYSEAMERICAN,
                _ => Unknown,
            };
        }

        public static ExchangeType ToExchange(int id)
        {
            switch (id)
            {
                case 1: return NASDAQ;
                case 2: return NYSE;
                case 3: return NYSEARCA;
                case 4: return NYSEAMERICAN;
                default:
                    return Unknown;
            }
        }
    }
}
