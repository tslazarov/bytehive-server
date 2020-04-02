using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Notifications.AppConfig
{
    public interface IAppConfiguration
    {
        string SendGridKey { get; set; }
    }
}
