using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.ConfigurationOptions
{
    public class FinnHubOptions
    {

        [Required]
        public string ApiKey { get; set; }

        [Required]
        public string ClientName { get; set; }

        [Url]
        public string Url { get; set; }

        [Range(5, 50)]
        public int SearchLimit { get; set; }

        public FinnHubOptions()
        {
        }
    }
}
