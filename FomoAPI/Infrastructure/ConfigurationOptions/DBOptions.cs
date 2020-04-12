using System.ComponentModel.DataAnnotations;

namespace FomoAPI.Infrastructure.ConfigurationOptions
{
    public class DbOptions
    {
        [Required]
        public string ConnectionString { get; set; }
    }
}
