using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.Users
{
    public class UserListViewModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public string Provider { get; set; }

        public DateTime RegistrationDate { get; set; }

        public int TotalRequests { get; set; }
        
        public Occupation Occupation { get; set; }
    }
}
