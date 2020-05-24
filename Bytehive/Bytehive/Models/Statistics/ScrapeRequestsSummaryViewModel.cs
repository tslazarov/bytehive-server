using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.Statistics
{
    public class ScrapeRequestsSummaryViewModel
    {
        public DateTime CreationDate { get; set; }

        public string Email { get; set; }

        public int Entries { get; set; }

        public ExportType ExportType { get; set; }
    }
}
