using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Scraper.Contracts
{
    public interface IScraperParser
    {
        HtmlNode GetNodeFromHtml(string content);

        string GetQuerySelectorFromText(string content, string text, string element = "", string elementName = "", bool scrapeLink = false, bool isUnique = true, int line = -1);

        string SanitizeHtml(string html, string host);

        bool ValidateListQuerySelector(string content, string markup);

        bool ValidateDetailQuerySelector(string content, List<Tuple<string, string>> mappings, ref List<Tuple<string, string>> mappingsResult);

        string CreateSelector(HtmlNode node);
    }
}
