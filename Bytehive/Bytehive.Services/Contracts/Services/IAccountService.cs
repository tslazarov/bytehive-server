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
        Task<CombinedToken> Authenticate(string email, string password, string remoteIpAddress);
    }
}
