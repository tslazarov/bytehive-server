using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Services.Dto
{
    public class CombinedToken
    {
        public AccessToken AccessToken { get; set; }

        public RefreshToken RefreshToken { get; set; }
    }
}
