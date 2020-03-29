using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.Account
{
    public class SigninExternalUserModel
    {
        public string Email { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
     
        public string Provider { get; set; }
        
        public string Token { get; set; }

        public int Occupation { get; set; }

        public int DefaultLanguage { get; set; }
    }
}
