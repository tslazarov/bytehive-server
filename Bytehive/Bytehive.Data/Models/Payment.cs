using Bytehive.Data.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bytehive.Data.Models
{
    [Table("payment")]
    public class Payment : IIdentifier
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }

        public string ExternalId { get; set; }

        public string Provider { get; set; }

        public PaymentStatus Status { get; set; }

        public decimal Price { get; set; }

        public User User { get; set; }

        public Guid UserId { get; set; }

        public PaymentTier PaymentTier { get; set; }

        public Guid PaymentTierId { get; set; }
    }
}
