using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FomoAPI.Application.ConfigurationOptions
{
    public class DemoUser
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
