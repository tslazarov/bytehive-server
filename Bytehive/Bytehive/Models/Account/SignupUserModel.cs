﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.Account
{
    public class SignupUserModel
    {
        public string Email { get; set; }
     
        public string Password { get; set; }
        
        public string ConfirmPassword { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public int Occupation { get; set; }

        public int DefaultLanguage { get; set; }
    }
}
