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
    [Route("api/[controller]")]
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
        public async Task<ActionResult> Markup(string url)
        {
            var response = await this.scraperClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            return new ContentResult() { StatusCode = StatusCodes.Status200OK, ContentType = "text/html", Content = content };
        }

        [HttpPut]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("code")]
        public async Task<ActionResult> Code(CodeModel model)
        {
            var response = await this.scraperClient.GetAsync(model.Url);
            var content = await response.Content.ReadAsStringAsync();

            var node = this.scraperParser.GetNodeFromMarkup(model.Text);
            var query = this.scraperParser.GetQuerySelectorFromText(content, node.InnerText, node.Name, model.Line);

            return new JsonResult(query) { StatusCode = StatusCodes.Status200OK };
        }
    }
}
