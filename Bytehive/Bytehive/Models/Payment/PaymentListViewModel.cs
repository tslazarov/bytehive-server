using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.Payment
{
    public class PaymentListViewModel
    {
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }

        public Guid UserId { get; set; }

        public string Email { get; set; }

        public string ExternalId { get; set; }

        public string Provider { get; set; }

        public PaymentStatus Status { get; set; }

        public decimal Price { get; set; }
    }
}
