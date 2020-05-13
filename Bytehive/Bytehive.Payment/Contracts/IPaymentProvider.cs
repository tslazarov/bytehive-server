using Bytehive.Data.Models;
using PayPalHttp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Payment.Contracts
{
    public interface IPaymentProvider
    {
        Task<object> CreateOrder(PaymentTier paymentTier);

        Task<object> AuthorizeOrder(string orderId);

        Task<object> GetOrder(string orderId);

        Task<object> VerifyOrder(string orderId);
    }
}
