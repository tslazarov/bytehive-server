using Bytehive.Payments.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Payments.PayPal
{
    public delegate IPaymentProvider PaymentProviderResolver(string key);

}
