using AutoMapper;
using Bytehive.Data.Models;
using Bytehive.Models.Scraper;
using Bytehive.Models.ScrapeRequests;
using Bytehive.Scraper;
using Bytehive.Scraper.Contracts;
using Bytehive.Services.Contracts.Services;
using Bytehive.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bytehive.Controllers
{
    [ApiController]
    [Route("api/scraperequests")]
    public class ScrapeRequestsController : Controller
    {
        private readonly IScrapeRequestsService scrapeRequestsService;
        private readonly IMapper mapper;

        public ScrapeRequestsController(IScrapeRequestsService scrapeRequestsService,
            IMapper mapper)
        {
            this.scrapeRequestsService = scrapeRequestsService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("all")]
        public async Task<ActionResult> All()
        {
            var scrapeRequests = await this.scrapeRequestsService.GetScrapeRequests<ScrapeRequestListViewModel>();

            return new JsonResult(scrapeRequests.OrderByDescending(i => i.CreationDate)) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("detail")]
        public async Task<ActionResult> Detail(string id)
        {
            return new JsonResult("") { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("create")]
        public async Task<ActionResult> Create(ScrapeRequestCreateModel model)
        {
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                Guid id;

                if (identity.FindFirst("id") != null && Guid.TryParse(identity.FindFirst("id").Value, out id))
                {
                    var scrapeRequest = this.mapper.Map<ScrapeRequest>(model);
                    scrapeRequest.UserId = id;

                    var requestCreated = await this.scrapeRequestsService.Create(scrapeRequest);

                    if (requestCreated)
                    {
                        return new JsonResult(true) { StatusCode = StatusCodes.Status200OK };
                    }
                }
            }

            return new JsonResult(false) { StatusCode = StatusCodes.Status400BadRequest };
        }

        [HttpDelete]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("delete")]
        public async Task<ActionResult> Delete(string id)
        {
            Guid parsedId;
            bool deleted = false;

            if (Guid.TryParse(id, out parsedId))
            {
                ScrapeRequest scrapeRequest = await this.scrapeRequestsService.GetScrapeRequest<ScrapeRequest>(parsedId);

                if (scrapeRequest != null)
                {
                    deleted = await this.scrapeRequestsService.Delete(scrapeRequest);

                }
            }

            var statusCode = deleted ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;

            return new JsonResult(deleted) { StatusCode = statusCode };
        }
    }
}
