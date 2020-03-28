using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Services.AppConfig
{
    public interface IAppConfiguration
    {
        string FacebookAppId { get; set; }

        string FacebookAppSecret { get; set; }

        string GoogleClientId { get; set; }
    }
}
