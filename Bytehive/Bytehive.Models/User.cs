using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bytehive.Models
{
    public class User : IDataItem
    {
        public User()
        {
        }

        public User(Guid id, string email, string firstName, string lastName, bool isExternal)
        {
            this.Id = id;
            this.Email = email;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.IsExternal = isExternal;
            this.ScrapeRequests = new List<ScrapeRequest>();
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

        public bool IsExternal { get; set; }

        public string UserExternalId { get; set; }

        public virtual ICollection<ScrapeRequest> ScrapeRequests { get; set; }
    }
}
