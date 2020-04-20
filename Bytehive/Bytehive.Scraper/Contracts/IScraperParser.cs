using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Scraper.Contracts
{
    public interface IScraperParser
    {
        HtmlNode GetNodeFromHtml(string content);

        string GetQuerySelectorFromText(string content, string text, string element, bool scrapeLink = false, int line = -1);

        string SanitizeHtml(string html, string host);

        bool ValidateListQuerySelector(string content, string markup);
    }
}
