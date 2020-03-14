using System;
using System.ComponentModel.DataAnnotations;

namespace Bytehive.Models
{
    public class ScrapeRequest
    {
        [Key]
        public Guid Id { get; set; }

        public User User { get; set; }

        public Guid UserId { get; set; }

        public ScrapeType ScrapeType { get; set; }

        public ExportType ExportType { get; set; }

        public string Data { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string ValidationKey { get; set; }

        public string DownloadUrl { get; set; }
    }
}
