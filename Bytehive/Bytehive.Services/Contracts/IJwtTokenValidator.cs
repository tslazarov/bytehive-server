using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Bytehive.Services.Contracts
{
    public interface IJwtTokenValidator
    {
        ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey);
    }
}
