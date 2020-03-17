using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Services.Contracts
{
    public interface ITokenFactory
    {
        string GenerateToken(int size = 32);
    }
}
