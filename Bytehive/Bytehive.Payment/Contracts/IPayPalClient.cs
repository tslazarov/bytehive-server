using PayPalHttp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Payment.Contracts
{
    public interface IPayPalClient
    {
        HttpClient Client();

        HttpClient Client(string refreshToken);
    }
}
