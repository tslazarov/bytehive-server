using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bytehive.Data.Models
{
    [Table("user")]
    public class User 
    {
        public User()
        {
        }

        public User(Guid id, string email, string firstName, string lastName, string provider)
        {
            this.Id = id;
            this.Email = email;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Provider = provider;
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [EmailAddress]
        public string Email { get; set; }

        [MinLength(3)]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [MinLength(3)]
        [MaxLength(30)]
        public string LastName { get; set; }

        public string Salt { get; set; }

        public string HashedPassword { get; set; }

        public Language DefaultLanguage { get; set; }

        public string ResetCode { get; set; }

        public string Image { get; set; }

        public string Provider { get; set; }

        public Occupation Occupation { get; set; }

        public DateTime RegistrationDate { get; set; }

        public virtual ICollection<ScrapeRequest> ScrapeRequests { get; set; } = new List<ScrapeRequest>();

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        [NotMapped]
        public int TotalRequests => ScrapeRequests.Count;

        public void AddRefreshToken(Guid id, string token, Guid userId, double daysToExpire = 5)
        {
            this.RefreshTokens.Add(new RefreshToken(id, token, DateTime.UtcNow.AddDays(daysToExpire), userId));
        }
    }
}
