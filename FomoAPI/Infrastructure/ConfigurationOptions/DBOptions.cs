using System.ComponentModel.DataAnnotations;

namespace FomoAPI.Infrastructure.ConfigurationOptions
{
    public class DbOptions
    {
        [Required]
        public string ConnectionString { get; set; }

        [Required]
        [Range(1, 10000)]
        public int DefaultBulkCopyBatchSize { get; set; }
    }
}
