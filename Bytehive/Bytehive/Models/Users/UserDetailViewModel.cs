using Bytehive.Data.Models;
using Bytehive.Models.Payment;
using Bytehive.Models.ScrapeRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.Users
{
    public class UserDetailViewModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public DateTime RegistrationDate { get; set; }
        
        public Occupation Occupation { get; set; }

        public Language DefaultLanguage { get; set; }

        public int Tokens { get; set; }

        public string Image { get; set; }

        public List<ScrapeRequestListViewModel> ScrapeRequests { get; set; }

        public List<PaymentListViewModel> Payments { get; set; }
    }
}
