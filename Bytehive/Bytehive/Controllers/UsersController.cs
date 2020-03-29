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
    [Route("api/[controller]")]
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
            //var usersList = await users.ProjectTo<UserListViewModel>(this.mapper.ConfigurationProvider)
            //    .ToListAsync();

            return new JsonResult(users) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("detail")]
        public async Task<ActionResult> Detail(string id)
        {
            return new JsonResult("") { StatusCode = StatusCodes.Status200OK };
        }
    }
}
