using AutoMapper;
using Bytehive.Data.Models;
using Bytehive.Models.Scraper;
using Bytehive.Models.ScrapeRequests;
using Bytehive.Scraper;
using Bytehive.Scraper.Contracts;
using Bytehive.Services.Contracts.Services;
using Bytehive.Services.Utilities;
using Bytehive.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bytehive.Controllers
{
    [ApiController]
    [Route("api/scraperequests")]
    public class ScrapeRequestsController : Controller
    {
        private readonly IScrapeRequestsService scrapeRequestsService;
        private readonly IAzureBlobStorageProvider azureBlobStorageProvider;
        private readonly IMapper mapper;

        public ScrapeRequestsController(IScrapeRequestsService scrapeRequestsService,
            IAzureBlobStorageProvider azureBlobStorageProvider,
            IMapper mapper)
        {
            this.scrapeRequestsService = scrapeRequestsService;
            this.azureBlobStorageProvider = azureBlobStorageProvider;
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
            Guid parsedId;

            if (Guid.TryParse(id, out parsedId))
            {
                ScrapeRequestDetailViewModel scrapeRequest = await this.scrapeRequestsService.GetScrapeRequest<ScrapeRequestDetailViewModel>(parsedId);

                return new JsonResult(scrapeRequest) { StatusCode = StatusCodes.Status200OK };
            }

            return new JsonResult(null) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet]
        [Route("file/{id}")]
        public async Task<ActionResult> DownloadFile(string id, string token)
        {
            Guid parsedId;
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;


            if (Guid.TryParse(id, out parsedId))
            {
                ScrapeRequest scrapeRequest = await this.scrapeRequestsService.GetScrapeRequest<ScrapeRequest>(parsedId);

                if (scrapeRequest != null && scrapeRequest.File != null)
                {
                    if((!string.IsNullOrEmpty(scrapeRequest.ValidationKey) && scrapeRequest.ValidationKey == token) || (identity != null && identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Role)?.Value == Constants.Strings.Roles.Administrator))
                    {
                        var file = await this.azureBlobStorageProvider.DownloadBlob(ScraperProcessor.FilesContainerName, scrapeRequest.File.Name);

                        var cd = new ContentDisposition { FileName = scrapeRequest.File.Name, Inline = false };
                        Response.Headers.Add("Content-Disposition", cd.ToString());

                        return File(file.Content, file.ContentType);
                    }
                }
                else
                {
                    return new JsonResult("File not found") { StatusCode = StatusCodes.Status404NotFound };
                }
            }

            return new JsonResult("Forbidden") { StatusCode = StatusCodes.Status403Forbidden };
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
