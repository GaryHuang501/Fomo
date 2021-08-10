using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.ConfigurationOptions
{
    public class AccountsOptions
    {
        [Required]
        public DemoUser DemoUser { get; set; }

        [Required]
        public string RegistrationPage { get; set; }

        [Required]
        public string MainPage { get; set; }
    }
}
