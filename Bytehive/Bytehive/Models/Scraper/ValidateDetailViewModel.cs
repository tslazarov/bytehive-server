using Bytehive.Models.ScrapeRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.Scraper
{
    public class ValidateDetailViewModel
    {
        public bool Valid { get; set; }

        public List<Tuple<string, string>> FieldMappings { get; set; }

        public ValidateDetailViewModel(bool valid, List<Tuple<string,string>> fieldMappings)
        {
            this.Valid = valid;
            this.FieldMappings = fieldMappings;
        }
    }
}
