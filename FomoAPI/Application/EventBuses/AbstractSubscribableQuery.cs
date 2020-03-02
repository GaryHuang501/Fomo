using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FomoAPI.Common;
using FomoAPI.Infrastructure.Enums;

namespace FomoAPI.Application.EventBuses
{
    public abstract class AbstractSubscribableQuery: ISubscribableQuery
    {
        public QueryFunctionType FunctionType { get; }

        public string Symbol { get; }

        public DateTime CreateDate { get; }

        protected AbstractSubscribableQuery(QueryFunctionType functionType, string symbol)
        {
            if (string.IsNullOrEmpty(symbol)) throw new ArgumentNullException(nameof(symbol));

            FunctionType = functionType;
            Symbol = symbol;
            CreateDate = DateTime.UtcNow;
        }

        public override bool Equals(object obj)
        {
            var otherQuery = obj as ISubscribableQuery;

            return otherQuery != null
                && otherQuery.Symbol == Symbol
                && otherQuery.FunctionType == FunctionType;
        }

        public override int GetHashCode()
        {
            return this.CalculateHashCodeForParams(Symbol, FunctionType);
        }
    }
}
