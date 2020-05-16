using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.Payment
{
    public class AuthorizeOrderModel
    {
        public string Provider { get; set; }

        public string OrderId { get; set; }
    }
}
