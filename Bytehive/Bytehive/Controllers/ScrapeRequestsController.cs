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
        private readonly IUsersService usersService;
        private readonly IAzureBlobStorageProvider azureBlobStorageProvider;
        private readonly IMapper mapper;

        public ScrapeRequestsController(IScrapeRequestsService scrapeRequestsService,
            IUsersService usersService,
            IAzureBlobStorageProvider azureBlobStorageProvider,
            IMapper mapper)
        {
            this.scrapeRequestsService = scrapeRequestsService;
            this.usersService = usersService;
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
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("all/profile")]
        public async Task<ActionResult> AllProfile()
        {
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                Guid id;

                if (identity.FindFirst("id") != null && Guid.TryParse(identity.FindFirst("id").Value, out id))
                {
                    var scrapeRequests = await this.scrapeRequestsService.GetScrapeRequests<ScrapeRequestProfileListViewModel>();
                    var filteredRequests = scrapeRequests.Where(r => r.UserId == id);

                    return new JsonResult(filteredRequests.OrderByDescending(i => i.CreationDate)) { StatusCode = StatusCodes.Status200OK };
                }
            }

            return new JsonResult(new List<object>()) { StatusCode = StatusCodes.Status200OK };
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
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("detail/profile")]
        public async Task<ActionResult> DetailProfile(string id)
        {
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                Guid userId;

                if (identity.FindFirst("id") != null && Guid.TryParse(identity.FindFirst("id").Value, out userId))
                {
                    Guid parsedId;

                    if (Guid.TryParse(id, out parsedId))
                    {
                        ScrapeRequestDetailViewModel scrapeRequest = await this.scrapeRequestsService.GetScrapeRequest<ScrapeRequestDetailViewModel>(parsedId);

                        if (scrapeRequest.UserId == userId)
                        {
                            return new JsonResult(scrapeRequest) { StatusCode = StatusCodes.Status200OK };
                        }
                        else
                        {
                            return new JsonResult(new object()) { StatusCode = StatusCodes.Status200OK };
                        }
                    }
                }
            }

            return new JsonResult(new object()) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet]
        [Route("file/{id}")]
        public async Task<ActionResult> DownloadFile(string id, string token)
        {
            Guid parsedId;
            Guid userId;
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;

            if (Guid.TryParse(id, out parsedId))
            {
                ScrapeRequest scrapeRequest = await this.scrapeRequestsService.GetScrapeRequest<ScrapeRequest>(parsedId);

                if (scrapeRequest != null && scrapeRequest.File != null)
                {
                    var hasKey = (!string.IsNullOrEmpty(scrapeRequest.AccessKey) && scrapeRequest.AccessKey == token);
                    var isAdministrator = (identity != null && identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Role)?.Value == Constants.Strings.Roles.Administrator);
                    var isOwner = (identity != null && Guid.TryParse(identity.FindFirst("id")?.Value, out userId) && userId == scrapeRequest.UserId);
                   
                    if (hasKey || isAdministrator || isOwner)
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

        [HttpPut]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("unlock")]
        public async Task<ActionResult> Unlock(string id)
        {
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                Guid userId;

                Guid parsedId;
                bool updated = false;

                if (Guid.TryParse(id, out parsedId) && identity.FindFirst("id") != null && Guid.TryParse(identity.FindFirst("id").Value, out userId))
                {
                    ScrapeRequest scrapeRequest = await this.scrapeRequestsService.GetScrapeRequest<ScrapeRequest>(parsedId);

                    if (scrapeRequest != null)
                    {
                        var reducedTokens = Math.Ceiling(scrapeRequest.Entries / 100.0);
                        var user = await this.usersService.GetUser<User>(userId);

                        if(user != null)
                        {
                            user.Tokens -= Convert.ToInt32(reducedTokens);
                            var userUpdated = await this.usersService.Update(user);
                        
                            if(userUpdated)
                            {
                                scrapeRequest.Status = ScrapeRequestStatus.Paid;
                                scrapeRequest.AccessKey = AccessKeyHelper.CreateAccessKey(16);
                                updated = await this.scrapeRequestsService.Update(scrapeRequest);
                            }
                        }
                    }
                }

                var statusCode = updated ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;

                return new JsonResult(updated) { StatusCode = statusCode };
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
