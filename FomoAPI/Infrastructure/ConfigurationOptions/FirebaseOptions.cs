using System;
using System.ComponentModel.DataAnnotations;

namespace FomoAPI.Infrastructure.ConfigurationOptions
{
    public class FirebaseOptions
    {
        public string ApiKey { get; set; }

        public string ClientName { get; set; }

        public string DatabaseUrl { get; set; }

        public string ServiceAccountCredentials { get; set; }

        public int TokenRenewalMinutes { get; set; }

        public bool AuthEnabled { get; set; }
    }
}
