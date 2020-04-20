using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.ScrapeRequests
{
    public class ScrapeRequestCreateModel
    {
        public ScrapeType ScrapeType { get; set; }

        public ExportType ExportType { get; set; }

        public string ListUrl { get; set; }

        public bool HasPaging { get; set; }

        public int? StartPage { get; set; }

        public int? EndPage { get; set; }

        public string DetailMarkup { get; set; }

        public List<string> DetailUrls { get; set; }

        public List<FieldMappingModel> FieldMappings { get; set; }
    }
}
