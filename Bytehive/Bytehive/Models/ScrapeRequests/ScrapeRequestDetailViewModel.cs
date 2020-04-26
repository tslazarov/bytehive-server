using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.ScrapeRequests
{
    public class ScrapeRequestDetailViewModel
    {
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public Guid UserId { get; set; }

        public string Email { get; set; }

        public string DownloadUrl { get; set; }

        public string Data { get; set; }

        public ScrapeType ScrapeType { get; set; }

        public ExportType ExportType { get; set; }

        public ScrapeRequestStatus Status { get; set; }
    }
}
