using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Services.Utilities
{
    public static class Constants
    {
        public static class Strings
        {
            public static class JwtClaimIdentifiers
            {
                public const string Role = "role";
                public const string Id = "id";
                public const string Email = "email";
            }

            public static class JwtClaims
            {
                public const string ApiAccess = "api_access";
                public const string ApiUser = "User";
                public const string ApiAdministrator = "Administrator";
            }
        }
    }
}
