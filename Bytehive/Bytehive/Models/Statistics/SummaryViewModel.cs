using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.Statistics
{
    public class SummaryViewModel
    {
        public long UsersCount { get; set; }
        
        public long RequestsCount { get; set; }

        public long EntriesCount { get; set; }

        public decimal PaymentsTotal { get; set; }
    }
}
