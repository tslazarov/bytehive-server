using Bytehive.Data.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bytehive.Data.Models
{
    public class RefreshToken : IIdentifier
    {
        [Key]
        public Guid Id { get; set; }

        public string Token { get; set; }

        public DateTime ExpirationDate { get; set; }

        public User User { get; set; }

        public Guid UserId { get; set; }
        
        [NotMapped]
        public bool Active => DateTime.UtcNow <= ExpirationDate;

        public RefreshToken(Guid id, string token, DateTime expirationDate, Guid userId)
        {
            Id = id;
            Token = token;
            ExpirationDate = expirationDate;
            UserId = userId;
        }
    }
}
