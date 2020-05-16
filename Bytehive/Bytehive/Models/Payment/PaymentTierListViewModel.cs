using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.Payment
{
    public class PaymentTierListViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Value { get; set; }

        public decimal Price { get; set; }
    }
}
