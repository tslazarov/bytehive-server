using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.Payment
{
    public class CreateOrderModel
    {
        public string Provider { get; set; }

        public string Tier { get; set; }
    }
}
