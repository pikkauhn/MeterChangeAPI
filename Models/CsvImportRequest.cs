using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MeterChangeApi.Models
{
    public class CsvImportRequest
    {
        [Required]
        public IFormFile File { get; set; } = null!;
        [Required]
        public ImportMode ImportMode { get; set; }
    }
}
