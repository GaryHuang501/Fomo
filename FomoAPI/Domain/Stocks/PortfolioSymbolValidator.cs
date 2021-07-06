using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks
{
    public class PortfolioSymbolValidator : AbstractValidator<PortfolioSymbol>
    {
        public PortfolioSymbolValidator()
        {
            RuleFor(ps => ps.AveragePrice).GreaterThanOrEqualTo(0);
        }
    }
}
