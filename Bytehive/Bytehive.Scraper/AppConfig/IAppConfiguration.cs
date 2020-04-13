using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Scraper.AppConfig
{
    public interface IAppConfiguration
    {
        string ScraperApiKey { get; set; }
    }
}
