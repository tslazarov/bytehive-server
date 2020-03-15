using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.Account
{
    public class SigninUserModel
    {
        public string Email { get; set; }
     
        public string Password { get; set; }

        public string RemoteIpAddress { get; set; }
    }
}
