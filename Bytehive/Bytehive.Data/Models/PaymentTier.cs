using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bytehive.Data.Models
{
    [Table("payment_tier")]
    public class PaymentTier
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Value { get; set; }

        public string Sku { get; set; }

        public decimal Price { get; set; }

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
