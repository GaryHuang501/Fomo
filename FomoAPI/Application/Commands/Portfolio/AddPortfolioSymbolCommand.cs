﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.Commands.Portfolio
{
    public class AddPortfolioSymbolCommand
    {
        public string Ticker { get; set; }

        public string Exchange { get; set; }
    }
}
