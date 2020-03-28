using Bytehive.Data.Models;
using Bytehive.Services.Dto;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Services
{
    public interface IAccountService
    {
        Task<CombinedToken> AuthenticateLocal(User user, string email, string password, string remoteIpAddress);

        Task<CombinedToken> AuthenticateExternal(string email, string firstName, string lastName, int occupation, int defaultLanguage, string providerName, string remoteIpAddress, string token);

        Task<bool> Unauthenticate(Guid id, string providerName);
    }
}
