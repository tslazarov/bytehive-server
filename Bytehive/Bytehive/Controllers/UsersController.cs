using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bytehive.Data.Models;
using Bytehive.Models.Users;
using Bytehive.Services.Contracts;
using Bytehive.Services.Contracts.Services;
using Bytehive.Services.Utilities;
using Bytehive.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bytehive.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly IUsersService usersService;
        private readonly IMapper mapper;

        public UsersController(IUsersService usersService,
            IMapper mapper)
        {
            this.usersService = usersService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Authorize(Policy=Constants.Strings.Roles.Administrator)]
        [Route("all")]
        public async Task<ActionResult> All()
        {
            var users = await this.usersService.GetUsers<UserListViewModel>();

            return new JsonResult(users) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("detail/{id}")]
        public async Task<ActionResult> Detail(string id)
        {
            Guid parsedId;

            if (Guid.TryParse(id, out parsedId))
            {
                var user = await this.usersService.GetUser<UserDetailViewModel>(parsedId);

                return new JsonResult(user) { StatusCode = StatusCodes.Status200OK };
            }

            return new JsonResult("") { StatusCode = StatusCodes.Status400BadRequest };
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
                User user = await this.usersService.GetUser<User>(parsedId);

                if(user != null)
                {
                    deleted = await this.usersService.Delete(user);
                
                }
            }

            var statusCode = deleted ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;

            return new JsonResult(deleted) { StatusCode = statusCode };
        }
    }
}
