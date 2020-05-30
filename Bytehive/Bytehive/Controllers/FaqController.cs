using Bytehive.Data.Models;
using Bytehive.Models.Statistics;
using Bytehive.Services.Contracts.Services;
using Bytehive.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Controllers
{
    [ApiController]
    [Route("api/faq")]
    public class FaqController : Controller
    {
        public FaqController()
        {
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
            return new JsonResult("") { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("detail")]
        public async Task<ActionResult> Detail(string id)
        {
            return new JsonResult("") { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("create")]
        public async Task<ActionResult> Create()
        {
            return new JsonResult("") { StatusCode = StatusCodes.Status200OK };
        }


        [HttpPut]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("edit")]
        public async Task<ActionResult> Edit()
        {
            return new JsonResult("") { StatusCode = StatusCodes.Status200OK };
        }

        [HttpDelete]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("delete")]
        public async Task<ActionResult> Delete()
        {
            return new JsonResult("") { StatusCode = StatusCodes.Status200OK };
        }
    }
}
