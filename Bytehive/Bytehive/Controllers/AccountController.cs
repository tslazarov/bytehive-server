using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Bytehive.Data.Models;
using Bytehive.Models.Account;
using Bytehive.Services.Contracts;
using Bytehive.Services.Contracts.Services;
using Bytehive.Services.Utilities;
using Bytehive.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Bytehive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IUsersService usersService;
        private readonly IAccountService accountService;
        private readonly IMapper mapper;

        public AccountController(IUsersService usersService,
            IAccountService accountService,
            IMapper mapper)
        {
            this.usersService = usersService;
            this.accountService = accountService;
            this.mapper = mapper;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("signup")]
        public async Task<ActionResult> SignUp(SignupUserModel model)
        {
            var existingUser = await this.usersService.GetUser(model.Email);

            if(existingUser != null)
            {
                return new JsonResult("A user with the corresponding email already exists.") { StatusCode = StatusCodes.Status400BadRequest };
            }

            var user = this.mapper.Map<User>(model);

            var salt = PasswordHelper.CreateSalt(10);
            var hashedPassword = PasswordHelper.CreatePasswordHash(model.Password, salt);

            user.Salt = salt;
            user.HashedPassword = hashedPassword;

            bool userCreated = false;

            try
            {
                userCreated = await this.usersService.Create(user);
            }
            catch(Exception e)
            {
                if(e.InnerException != null && e.InnerException is SqlException)
                {
                    var sqlException = e.InnerException as SqlException;

                    if(sqlException.Number == 2601 && sqlException.Message.ToLower().Contains("email"))
                    {
                    }
                }
            }

            if (userCreated)
            {
                return new JsonResult("The user was created.") { StatusCode = StatusCodes.Status201Created };
            }

            return new JsonResult("An error occured during user creation.") { StatusCode = StatusCodes.Status500InternalServerError };
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("signin")]
        public async Task<ActionResult> SignIn(SigninUserModel model)
        {
            var token = await this.accountService.Authenticate(model.Email, model.Password, model.RemoteIpAddress);

            if(token == null)
            {
                return new JsonResult("Ivalid username or password") { StatusCode = StatusCodes.Status400BadRequest };
            }

            return new JsonResult(token) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("signout")]
        public async Task<ActionResult> SignOut()
        {
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;
            
            if(identity != null)
            {
                Guid id;
                
                if(identity.FindFirst("id") != null && Guid.TryParse(identity.FindFirst("id").Value, out id))
                {
                    bool result = await this.accountService.Unauthenticate(id);

                    return new JsonResult(result) { StatusCode = StatusCodes.Status200OK };
                }
            }

            return new JsonResult(true) { StatusCode = StatusCodes.Status200OK };
        }
    }
}
