using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.ScrapeRequests
{
    public class ScrapeRequestListViewModel
    {
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }

        public Guid UserId { get; set; }

        public string Email { get; set; }

        public string DownloadUrl { get; set; }

        public ScrapeRequestStatus Status { get; set; }
    }
}
