using Bytehive.Scraper.Contracts;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bytehive.Scraper
{
    public class ScraperParser : IScraperParser
    {
        public ScraperParser()
        {

        }

        public HtmlNode GetNodeFromMarkup(string markup)
        {
            var html = new HtmlDocument();
            html.LoadHtml(markup);

            return html.DocumentNode.FirstChild;
        }

        public string GetQuerySelectorFromText(string markup, string text, string element = "", int line = -1)
        {
            var querySelector = string.Empty;
            var html = new HtmlDocument();
            html.LoadHtml(markup);
            var rootNode = html.DocumentNode;

            // TODO: Work on string exterpolation
            var textSelector = $"//*[contains(text(), '{text}')]";
            var nodes = rootNode.SelectNodes(textSelector);

            if (nodes == null || nodes.Count == 0 || (nodes.Count > 1 && line == -1))
            {
                return "non-determined";
            }

            var selectedNode = nodes.Count == 1 ? nodes[0] : GetNode(nodes, element, line);

            if(selectedNode != null)
            {
                while (true)
                {
                    querySelector = querySelector == string.Empty ? $"{CreateSelector(selectedNode)}" : $"{CreateSelector(selectedNode)} > {querySelector}";

                    if (selectedNode.ParentNode == null || IsUniqueSelector(rootNode, querySelector))
                    {
                        break;
                    }

                    selectedNode = selectedNode.ParentNode;
                }
            }

            return selectedNode == null ? "non-determined" : selectedNode.ParentNode == null ? "non-unique" : string.IsNullOrEmpty(querySelector) ? "non-determined" : querySelector;
        }

        private string CreateSelector(HtmlNode node)
        {
            var element = node.Name;

            foreach (var attribute in node.Attributes)
            {
                // TODO: add blacklist for other attributes if needed
                if(!ShouldProcessAttribute(attribute.Name))
                {
                    continue;
                }

                element += attribute.Name == "id" ? $"#{attribute.Value}" : attribute.Name == "class" ? $".{attribute.Value.Replace(" ", ".")}" : $"[{attribute.Name}='{attribute.Value}']";
            }

            return element;
        }

        private HtmlNode GetNode(HtmlNodeCollection nodes, string element, int line)
        {
            var elementNodes = element == "#text" ? nodes.Where(n => n.Line == line) : nodes.Where(n => n.Name == element);
            return elementNodes.Count() > 1 ? nodes.FirstOrDefault(n => n.Line == line) : elementNodes.FirstOrDefault();
        }

        private bool IsUniqueSelector(HtmlNode rootNode, string selector)
        {
            var nodes = rootNode.QuerySelectorAll(selector).ToList();
            return nodes.Count == 1 ? true : false;
        }

        private bool ShouldProcessAttribute(string name)
        {
            return whiteListAttributes.Any(attr => name.Contains(attr));
        }

        private readonly List<string> whiteListAttributes = new List<string>() { "id", "class", "name", "placeholder", "label", "data-*" };
    }
}
