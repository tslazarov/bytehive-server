using PayPalHttp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Payment.Contracts
{
    public interface IPaymentProvider
    {
        Task<object> CreateOrder();

        Task<object> AuthorizeOrder(string orderId);
    }
}
