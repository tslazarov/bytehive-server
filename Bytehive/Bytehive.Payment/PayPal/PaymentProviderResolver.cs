using Bytehive.Payment.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Payment.PayPal
{
    public delegate IPaymentProvider PaymentProviderResolver(string key);

}
