using Bytehive.Data.Models;
using PayPalHttp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Payment.Contracts
{
    public interface IOrdersService
    {
        Task<object> CreateOrder(string providerName, PaymentTier paymentTier);

        Task<object> AuthorizeOrder(string providerName, string orderId);

        Task<object> GetOrder(string providerName, string orderId);

        Task<object> VerifyOrder(string providerName, string orderId);
    }
}
