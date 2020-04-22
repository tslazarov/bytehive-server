using Bytehive.Models.ScrapeRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.Scraper
{
    public class ValidateDetailModel
    {
        public string Url { get; set; }

        public List<FieldMappingModel> FieldMappings { get; set; }
    }
}
