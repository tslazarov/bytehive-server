using Bytehive.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts
{
    public interface IJwtFactory
    {
        Task<AccessToken> GenerateEncodedToken(string id, string email);
    }
}
