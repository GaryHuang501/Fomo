using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FomoAPI.Domain.Login
{
    public class UserValidationOptions
    {
        [Range(3, 253)]
        public int MinLength { get; set; }

        [Range(3, 256)]
        public int MaxLength { get; set; }
    }
}
