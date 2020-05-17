using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.Payment
{
    public class PaymentProfileListViewModel
    {
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }

        public PaymentStatus Status { get; set; }

        public decimal Price { get; set; }

        public string TierName { get; set; }

        public int TierValue { get; set; }
    }
}
