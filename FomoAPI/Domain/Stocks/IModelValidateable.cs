using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks
{
    public interface IModelValidateable
    {
        bool IsValid();
    }
}
