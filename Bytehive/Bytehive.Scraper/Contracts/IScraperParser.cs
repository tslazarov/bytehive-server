using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Scraper.Contracts
{
    public interface IScraperParser
    {
        HtmlNode GetNodeFromMarkup(string markup);

        string GetQuerySelectorFromText(string markup, string text, string element, bool scrapeLink = false, int line = -1);

        string SanitizeHtml(string html, string host);
    }
}
