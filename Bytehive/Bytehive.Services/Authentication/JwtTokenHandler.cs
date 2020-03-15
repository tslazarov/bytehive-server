using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Bytehive.Services.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace Bytehive.Services.Authentication
{
    public sealed class JwtTokenHandler : IJwtTokenHandler
    {
        private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler;

        public JwtTokenHandler()
        {
            if (this.jwtSecurityTokenHandler == null)
                this.jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        public string WriteToken(JwtSecurityToken jwt)
        {
            return this.jwtSecurityTokenHandler.WriteToken(jwt);
        }

        public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
        {
            try
            {
                var principal = this.jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
