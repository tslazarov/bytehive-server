using PayPalHttp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Payment.Contracts
{
    public interface IPaymentService
    {
        Task<object> CreateOrder(string providerName);

        Task<object> AuthorizeOrder(string providerName, string orderId);
    }
}
