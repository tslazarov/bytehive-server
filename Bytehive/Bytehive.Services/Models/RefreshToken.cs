using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Services.Dto
{
    public sealed class RefreshToken
    {
        public string Token { get; }

        public RefreshToken(string token)
        {
            Token = token;
        }
    }
}
