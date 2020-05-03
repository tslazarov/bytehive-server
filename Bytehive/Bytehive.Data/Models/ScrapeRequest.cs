using Bytehive.Data.Contracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bytehive.Data.Models
{
    [Table("scrape_request")]
    public class ScrapeRequest : IIdentifier
    {
        [Key]
        public Guid Id { get; set; }

        public User User { get; set; }

        public Guid UserId { get; set; }

        public ScrapeType ScrapeType { get; set; }

        public ExportType ExportType { get; set; }

        public ScrapeRequestStatus Status { get; set; }

        public string Data { get; set; }

        public DateTime ExpirationDate { get; set; }

        public DateTime CreationDate { get; set; }

        public string ValidationKey { get; set; }

        public string FileName { get; set; }
    }
}
