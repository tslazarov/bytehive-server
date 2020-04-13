using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Scraper.Contracts
{
    public interface IScraperParser
    {
        HtmlNode GetNodeFromMarkup(string markup);

        string GetQuerySelectorFromText(string markup, string text, string element, int line = -1);
    }
}
