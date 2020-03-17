using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bytehive.Data.Models
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }

        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public User User { get; set; }

        public Guid UserId { get; set; }
        
        [NotMapped]
        public bool Active => DateTime.UtcNow <= Expires;
        
        public string RemoteIpAddress { get; set; }

        public RefreshToken(Guid id, string token, DateTime expires, Guid userId, string remoteIpAddress)
        {
            Id = id;
            Token = token;
            Expires = expires;
            UserId = userId;
            RemoteIpAddress = remoteIpAddress;
        }
    }
}
