using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts
{
    public interface IExternalTokenValidator
    {
        Task<bool> ValidateFacebook(string token);

        Task<bool> ValidateGoogle(string token);
    }
}
