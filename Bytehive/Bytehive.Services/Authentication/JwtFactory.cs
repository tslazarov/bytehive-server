﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Bytehive.Data.Models;
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

        public async Task<AccessToken> GenerateEncodedToken(User user, string roles)
        {
            var identity = GenerateClaimsIdentity(user, roles);

            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                 new Claim(JwtRegisteredClaimNames.Jti, await this.jwtOptions.JtiGenerator()),
                 new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(this.jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                 identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Role),
                 identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Id),
                 identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Email),
                 identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Language),
                 identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Name),
                 identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Provider)
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

        private static ClaimsIdentity GenerateClaimsIdentity(User user, string roles)
        {
            return new ClaimsIdentity(new GenericIdentity(user.Email, "Token"), new[]
            {
                new Claim(Constants.Strings.JwtClaimIdentifiers.Id, user.Id.ToString()),
                new Claim(Constants.Strings.JwtClaimIdentifiers.Email, user.Email),
                new Claim(Constants.Strings.JwtClaimIdentifiers.Language, user.DefaultLanguage.ToString()),
                new Claim(Constants.Strings.JwtClaimIdentifiers.Provider, user.Provider),
                new Claim(Constants.Strings.JwtClaimIdentifiers.Name, string.Format("{0} {1}", user.FirstName, user.LastName)),
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
