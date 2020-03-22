using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Bytehive.Services.Contracts;
using Bytehive.Services.Dto;
using Bytehive.Services.Utilities;
using Microsoft.Extensions.Options;

namespace Bytehive.Services.Authentication
{
    public sealed class JwtFactory : IJwtFactory
    {
        private readonly IJwtTokenHandler jwtTokenHandler;
        private readonly JwtIssuerOptions jwtOptions;

        public JwtFactory(IJwtTokenHandler jwtTokenHandler, IOptions<JwtIssuerOptions> jwtOptions)
        {
            this.jwtTokenHandler = jwtTokenHandler;
            this.jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(this.jwtOptions);
        }

        public async Task<AccessToken> GenerateEncodedToken(string id, string email, string roles)
        {
            var identity = GenerateClaimsIdentity(id, email, roles);

            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub, email),
                 new Claim(JwtRegisteredClaimNames.Jti, await this.jwtOptions.JtiGenerator()),
                 new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(this.jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                 identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Role),
                 identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Id),
                 identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Email)
             };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                this.jwtOptions.Issuer,
                this.jwtOptions.Audience,
                claims,
                this.jwtOptions.NotBefore,
                this.jwtOptions.Expiration,
                this.jwtOptions.SigningCredentials);

            return new AccessToken(this.jwtTokenHandler.WriteToken(jwt), (int)this.jwtOptions.ValidFor.TotalSeconds);
        }

        private static ClaimsIdentity GenerateClaimsIdentity(string id, string email, string roles)
        {
            return new ClaimsIdentity(new GenericIdentity(email, "Token"), new[]
            {
                new Claim(Constants.Strings.JwtClaimIdentifiers.Id, id),
                new Claim(Constants.Strings.JwtClaimIdentifiers.Email, email),
                new Claim(Constants.Strings.JwtClaimIdentifiers.Role, roles)
            });
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }
    }
}
