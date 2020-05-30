using AutoMapper;
using Bytehive.Data.Models;
using Bytehive.Models.Faq;
using Bytehive.Models.Statistics;
using Bytehive.Services.Contracts.Services;
using Bytehive.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bytehive.Controllers
{
    [ApiController]
    [Route("api/faq")]
    public class FaqController : Controller
    {
        private readonly IFaqsService faqsService;
        private readonly IFaqCategoriesService faqCategoriesService;
        private readonly IMapper mapper;

        public FaqController(IFaqsService faqsService,
            IFaqCategoriesService faqCategoriesService,
            IMapper mapper)
        {
            this.faqsService = faqsService;
            this.faqCategoriesService = faqCategoriesService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("all")]
        public async Task<ActionResult> All()
        {
            return new JsonResult("") { StatusCode = StatusCodes.Status200OK };
        }


        [HttpGet]
        [Route("all/category")]
        public async Task<ActionResult> AllCategory()
        {
            var faqCategories = await this.faqCategoriesService.GetFaqCategories<FaqCategoryListViewModel>();

            return new JsonResult(faqCategories) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("detail/{id}")]
        public async Task<ActionResult> Detail(string id)
        {
            return new JsonResult("") { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("create")]
        public async Task<ActionResult> Create(FaqCreateModel model)
        {
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                Guid id;

                if (identity.FindFirst("id") != null && Guid.TryParse(identity.FindFirst("id").Value, out id))
                {
                    var faq = this.mapper.Map<FAQ>(model);

                    var faqCreated = await this.faqsService.Create(faq);

                    if (faqCreated)
                    {
                        return new JsonResult(true) { StatusCode = StatusCodes.Status200OK };
                    }
                }
            }

            return new JsonResult(false) { StatusCode = StatusCodes.Status400BadRequest };
        }


        [HttpPut]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("edit/{id}")]
        public async Task<ActionResult> Edit()
        {
            return new JsonResult("") { StatusCode = StatusCodes.Status200OK };
        }

        [HttpDelete]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("delete/{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            Guid parsedId;
            bool deleted = false;

            if (Guid.TryParse(id, out parsedId))
            {
                FAQ faq = await this.faqsService.GetFaq<FAQ>(parsedId);

                if (faq != null)
                {
                    deleted = await this.faqsService.Delete(faq);
                }
            }

            var statusCode = deleted ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;

            return new JsonResult(deleted) { StatusCode = statusCode };
        }
    }
}
