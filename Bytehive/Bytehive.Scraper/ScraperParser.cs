using Bytehive.Scraper.Contracts;
using Bytehive.Scraper.Utilites;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
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


        public HtmlNode GetNodeFromHtml(string content)
        {
            var html = new HtmlDocument();
            html.LoadHtml(content);

            return html.DocumentNode.FirstChild;
        }

        public string GetQuerySelectorFromText(string content, string text, string element = "", string elementName = "", bool scrapeLink = false, int line = -1)
        {
            var querySelector = string.Empty;
            var html = new HtmlDocument();
            html.LoadHtml(content);
            var rootNode = html.DocumentNode;

            var nodes = rootNode.QuerySelectorAll(element)?.ToList();

            if (nodes == null || nodes.Count != 1)
            {
                var xpathExpression = this.CreateXPathExpression(text);
                nodes = rootNode.SelectNodes(xpathExpression)?.ToList();

                if (IsNodeNoneDetermined(nodes, line, elementName, scrapeLink))
                {
                    return "non-determined";
                }
            }

            var selectedNode = nodes.Count == 1 ? nodes[0] : GetNode(nodes, elementName, line, scrapeLink);

            if(selectedNode != null)
            {
                while (true)
                {
                    querySelector = querySelector == string.Empty ? $"{CreateSelector(selectedNode)}" : scrapeLink ? $"{CreateSelector(selectedNode)}" : $"{CreateSelector(selectedNode)} > {querySelector}";

                    if (selectedNode.ParentNode == null || (!scrapeLink && IsUniqueSelector(rootNode, querySelector)) || (scrapeLink && selectedNode.Name == "a"))
                    {
                        break;
                    }

                    selectedNode = selectedNode.ParentNode;
                }
            }

            return selectedNode == null ? "non-determined" : selectedNode.ParentNode == null ? "non-unique" : string.IsNullOrEmpty(querySelector) ? "non-determined" : querySelector;
        }

        public bool ValidateListQuerySelector(string content, string markup)
        {
            var querySelector = string.Empty;
            var html = new HtmlDocument();
            html.LoadHtml(content);
            var rootNode = html.DocumentNode;

            var nodes = rootNode.QuerySelectorAll(markup).ToList();
            return nodes.Count > 0;
        }

        public bool ValidateDetailQuerySelector(string content, List<Tuple<string, string>> mappings, ref List<Tuple<string, string>> mappingsResult)
        {
            var isValid = true;
            var querySelector = string.Empty;
            var html = new HtmlDocument();
            html.LoadHtml(content);
            var rootNode = html.DocumentNode;

            foreach (var mapping in mappings)
            {
                var node = rootNode.QuerySelectorAll(mapping.Item2).FirstOrDefault();

                if(node != null)
                {
                    mappingsResult.Add(new Tuple<string, string>(mapping.Item1, HttpUtility.HtmlDecode(node.InnerText.Trim())));
                }
                else
                {
                    mappingsResult.Add(new Tuple<string, string>(mapping.Item1, string.Empty));
                    isValid = false;
                }
            }

            return isValid;
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

        public string CreateSelector(HtmlNode node)
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

        private HtmlNode GetNode(IList<HtmlNode> nodes, string elementName, int line, bool scrapeLink)
        {
            // used for line deviation as a result of whitespace normalization
            var lineStep = 5;

            var elementNodes = elementName == "#text" ? nodes.Where(n => (n.Line - lineStep < line && n.Line + lineStep > line)) : nodes.Where(n => n.Name == elementName);

            return elementNodes.FirstOrDefault(n => n.Name == "a") != null ? elementNodes.FirstOrDefault(n => n.Name == "a") : (elementNodes.Count() > 1 && line != -1) ? nodes.FirstOrDefault(n => n.Line == line) : elementNodes.FirstOrDefault();
        }

        private bool IsUniqueSelector(HtmlNode rootNode, string selector)
        {
            var nodes = rootNode.QuerySelectorAll(selector).ToList();
            return nodes.Count == 1;
        }

        private bool IsNodeNoneDetermined(IList<HtmlNode> nodes, int line, string element, bool scrapeLink)
        {
            return nodes == null || nodes.Count == 0 || (nodes.Count > 1 && line == -1 && !scrapeLink && (string.IsNullOrEmpty(element) || nodes.Where(n => n.Name == element).Count() > 1));
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

        private readonly List<string> whiteListAttributes = new List<string>() { "id", "class", "name", "placeholder", "label", "data-*", "itemprop" };
    }
}
