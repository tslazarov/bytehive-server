using Bytehive.Payment.Contracts;
using Bytehive.Payment.PayPal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Payment
{
    public class PaymentService : IPaymentService
    {
        IPaymentProvider provider;
        PaymentProviderResolver providerResolver;

        public PaymentService(PaymentProviderResolver providerResolver)
        {
            this.providerResolver = providerResolver;
        }

        public async Task<object> CreateOrder(string providerName)
        {
            this.provider = this.providerResolver(providerName);

            var order = await this.provider.CreateOrder();

            return order;
        }
    }
}
