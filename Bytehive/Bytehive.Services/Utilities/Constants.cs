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
                public const string Language = "language";
                public const string Name = "name";
                public const string Provider = "provider";
            }

            public static class JwtClaims
            {
                public const string ApiAccess = "api_access";
                public const string ApiUser = "User";
                public const string ApiAdministrator = "Administrator";
            }

            public static class UserProviders
            {
                public const string DefaultProvider = "Default";
                public const string FacebookProvider = "Facebook";
                public const string GoogleProvider = "Google";
            }

            public static class Roles
            {
                public const string User = "User";
                public const string Administrator = "Administrator";
            }
        }
    }
}
