using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Payments.AppConfig
{
    public interface IAppConfiguration
    {
        string PayPalClientId { get; set; }

        string PayPalClientSecret { get; set; }
    }
}
