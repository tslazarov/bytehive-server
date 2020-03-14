using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Bytehive.Data.Models;
using Bytehive.Services.Contracts;
using Bytehive.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bytehive.Controllers
{
    [ApiController]
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
        public async Task<HttpResponseMessage> Get()
        {
            var users = await this.usersService.GetAll<User>();

            return ResponseHelper.CreateJsonResponseMessage(users);
        }
    }
}
