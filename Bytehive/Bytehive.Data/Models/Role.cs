using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bytehive.Data.Models
{
    public class Role
    {
        public Role()
        {
        }

        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
