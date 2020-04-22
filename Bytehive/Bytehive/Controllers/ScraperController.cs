using Bytehive.Models.Scraper;
using Bytehive.Scraper;
using Bytehive.Scraper.Contracts;
using Bytehive.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Controllers
{
    [ApiController]
    [Route("api/scraper")]
    public class ScraperController: Controller
    {
        private readonly IScraperClient scraperClient;
        private readonly IScraperParser scraperParser;

        public ScraperController(IScraperClient scraperClient,
            IScraperParser scraperParser)
        {
            this.scraperClient = scraperClient;
            this.scraperParser = scraperParser;
        }

        [HttpGet]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("markup")]
        public async Task<ActionResult> Markup(string url, bool sanitize)
        {
            var response = await this.scraperClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var uri = new Uri(url);
            var host = string.Format("{0}://{1}", uri.Scheme, uri.Host);

            var sanitizedContent = sanitize ? this.scraperParser.SanitizeHtml(content, host) : content;

            return new ContentResult() { StatusCode = StatusCodes.Status200OK, ContentType = "text/html", Content = sanitizedContent };
        }

        [HttpPut]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("automatic")]
        public async Task<ActionResult> Automatic(AutomaticModel model)
        {
            var response = await this.scraperClient.GetAsync(model.Url);
            var content = await response.Content.ReadAsStringAsync();

            var node = this.scraperParser.GetNodeFromHtml(model.Text);
            var query = this.scraperParser.GetQuerySelectorFromText(content, node.InnerText, string.Empty, node.Name, model.ScrapeLink);

            return new JsonResult(query) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPut]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("visual")]
        public async Task<ActionResult> Visual(VisualModel model)
        {
            var response = await this.scraperClient.GetAsync(model.Url);
            var content = await response.Content.ReadAsStringAsync();

            var node = this.scraperParser.GetNodeFromHtml(model.Element);
            var querySelector = string.Empty;
            
            if(node != null)
            {
                querySelector = this.scraperParser.CreateSelector(node);
            }

            var query = this.scraperParser.GetQuerySelectorFromText(content, model.Text, querySelector, model.ElementName, model.ScrapeLink);

            return new JsonResult(query) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPut]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("code")]
        public async Task<ActionResult> Code(CodeModel model)
        {
            var response = await this.scraperClient.GetAsync(model.Url);
            var content = await response.Content.ReadAsStringAsync();

            var node = this.scraperParser.GetNodeFromHtml(model.Text);
            var querySelector = this.scraperParser.CreateSelector(node);
            var query = this.scraperParser.GetQuerySelectorFromText(content, node.InnerText, querySelector, node.Name, model.ScrapeLink, model.Line);

            return new JsonResult(query) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPut]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("validatelist")]
        public async Task<ActionResult> ValidateList(ValidateListModel model)
        {
            var response = await this.scraperClient.GetAsync(model.Url);
            var content = await response.Content.ReadAsStringAsync();

            var isValid = this.scraperParser.ValidateListQuerySelector(content, model.Markup);

            return new JsonResult(isValid) { StatusCode = StatusCodes.Status200OK };
        }


        [HttpPut]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("validatedetail")]
        public async Task<ActionResult> ValidateDetail(ValidateDetailModel model)
        {
            var response = await this.scraperClient.GetAsync(model.Url);
            var content = await response.Content.ReadAsStringAsync();
            var mappings = model.FieldMappings.Select(fm => new Tuple<string, string>(fm.FieldName, fm.FieldMarkup)).ToList();
            var mappingsResult = new List<Tuple<string, string>>();

            var isValid = this.scraperParser.ValidateListQuerySelector(content, mappings, ref mappingsResult);

            return new JsonResult(new ValidateDetailViewModel(isValid, mappingsResult)) { StatusCode = StatusCodes.Status200OK };
        }
    }
}
