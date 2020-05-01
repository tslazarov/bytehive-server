using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Storage.AppConfig
{
    public interface IAppConfiguration
    {
        string AzureBlobConnectionString { get; set; }
    }
}
