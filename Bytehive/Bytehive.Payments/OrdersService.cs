using Bytehive.Data.Models;
using Bytehive.Payments.Contracts;
using Bytehive.Payments.PayPal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Payments
{
    public class OrdersService : IOrdersService
    {
        IPaymentProvider provider;
        PaymentProviderResolver providerResolver;

        public OrdersService(PaymentProviderResolver providerResolver)
        {
            this.providerResolver = providerResolver;
        }

        public async Task<object> CreateOrder(string providerName, PaymentTier paymentTier)
        {
            this.provider = this.providerResolver(providerName);

            var order = await this.provider.CreateOrder(paymentTier);

            return order;
        }

        public async Task<object> AuthorizeOrder(string providerName, string orderId)
        {
            this.provider = this.providerResolver(providerName);

            var order = await this.provider.AuthorizeOrder(orderId);

            return order;
        }

        public async Task<object> GetOrder(string providerName, string orderId)
        {
            this.provider = this.providerResolver(providerName);

            var order = await this.provider.GetOrder(orderId);

            return order;
        }

        public async Task<object> VerifyOrder(string providerName, string orderId)
        {
            this.provider = this.providerResolver(providerName);

            var order = await this.provider.VerifyOrder(orderId);

            return order;
        }
    }
}
