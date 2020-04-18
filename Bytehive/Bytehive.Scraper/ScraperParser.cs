using Bytehive.Scraper.Contracts;
using Bytehive.Scraper.Utilites;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Bytehive.Scraper
{
    public class ScraperParser : IScraperParser
    {
        public ScraperParser()
        {

        }

        public string SanitizeHtml(string content, string host)
        {
            var html = new HtmlDocument();
            html.LoadHtml(content);

            html.DocumentNode.Descendants()
                    .Where(n => n.Name == "script")
                    .ToList()
                    .ForEach(n => n.Remove());

            this.TransformRelativeToAbsolute(host, html);

            return html.DocumentNode.OuterHtml;
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

            var xpathExpression = this.CreateXPathExpression(text);
            var nodes = rootNode.SelectNodes(xpathExpression);

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

        private string TransformRelativeToAbsolute(string host, HtmlDocument html)
        {
            foreach (var node in html.DocumentNode.Descendants())
            {
                if (node.Name == "img" && node.Attributes["src"] != null)
                {
                    if (!node.Attributes["src"].Value.StartsWith("http"))
                    {
                        var baseUri = new Uri(host);
                        var href = node.Attributes["src"].Value;
                        var relativeUrl = href.StartsWith("~/") ? href.Substring(2, href.Length - 2) : href.StartsWith("/") ? href.Substring(1, href.Length - 1) : href;

                        node.Attributes["src"].Value = new Uri(baseUri, relativeUrl).AbsoluteUri;
                    }
                }

                if((node.Name == "a" || node.Name == "link") && node.Attributes["href"] != null)
                {
                    if(!node.Attributes["href"].Value.StartsWith("http"))
                    {
                        var baseUri = new Uri(host);
                        var href = node.Attributes["href"].Value;
                        var relativeUrl = href.StartsWith("~/") ? href.Substring(2, href.Length - 2) : href.StartsWith("/") ? href.Substring(1, href.Length - 1) : href;

                        node.Attributes["href"].Value = new Uri(baseUri, relativeUrl).AbsoluteUri;
                    }
                }
            }

            return html.DocumentNode.OuterHtml;
        }

        private string CreateSelector(HtmlNode node)
        {
            var element = node.Name;

            foreach (var attribute in node.Attributes)
            {
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
            // used for line deviation as a result of whitespace normalization
            var lineStep = 5;

            var elementNodes = element == "#text" ? nodes.Where(n => (n.Line - lineStep < line && n.Line + lineStep > line)) : nodes.Where(n => n.Name == element);
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

        // Used against XPath injection
        private XPathExpression CreateXPathExpression(string text)
        {
            // hack for case insensitive search
            var lowerChars = string.Join("", text.ToCharArray().Distinct()).ToLower();
            var upperChars = lowerChars.ToUpper();

            // TODO: Work on HTML tag stripping - https://html-agility-pack.net/knowledge-base/26744559/htmlagilitypack--xpath-and-regex

            string xpathExpression = $"//*[contains(normalize-space(translate(text(), $upperChars, $lowerChars)), $text)]";
            XPathExpression xpath = XPathExpression.Compile(xpathExpression);

            // Arguments are provided as a XsltArgumentList()
            XsltArgumentList varList = new XsltArgumentList();
            varList.AddParam("text", string.Empty, text.ToLower());
            varList.AddParam("lowerChars", string.Empty, lowerChars);
            varList.AddParam("upperChars", string.Empty, upperChars);

            BytehiveScraperContext context = new BytehiveScraperContext(new NameTable(), varList);
            xpath.SetContext(context);

            return xpath;
        }

        private readonly List<string> whiteListAttributes = new List<string>() { "id", "class", "name", "placeholder", "label", "data-*" };
    }
}
