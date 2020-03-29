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
            var existingUser = await this.usersService.GetUser(model.Email, Constants.Strings.UserProviders.DefaultProvider);

            if(existingUser != null)
            {
                return new JsonResult("A user with the corresponding email already exists.") { StatusCode = StatusCodes.Status400BadRequest };
            }

            var user = this.mapper.Map<User>(model);

            var salt = PasswordHelper.CreateSalt(10);
            var hashedPassword = PasswordHelper.CreatePasswordHash(model.Password, salt);

            user.Salt = salt;
            user.HashedPassword = hashedPassword;
            user.Provider = "Default";

            var userCreated = await this.usersService.Create(user);
            var roleAssigned = await this.usersService.AssignRole(user.Id, Constants.Strings.Roles.User);

            if (userCreated && roleAssigned)
            {
                var token = await this.accountService.AuthenticateLocal(user, model.Email, model.Password);

                return new JsonResult(token) { StatusCode = StatusCodes.Status200OK };
            }

            return new JsonResult("An error occured during user creation.") { StatusCode = StatusCodes.Status500InternalServerError };
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("signin")]
        public async Task<ActionResult> SignIn(SigninUserModel model)
        {
            var token = await this.accountService.AuthenticateLocal(null, model.Email, model.Password);

            if(token == null)
            {
                return new JsonResult("Ivalid username or password") { StatusCode = StatusCodes.Status400BadRequest };
            }

            return new JsonResult(token) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("refreshtoken")]
        public async Task<ActionResult> RefreshToken(RefreshTokenModel model)
        {
            var token = await this.accountService.RefreshToken(model.Token);

            if (token == null)
            {
                return new JsonResult("Refresh token has expired") { StatusCode = StatusCodes.Status400BadRequest };
            }

            return new JsonResult(token) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("signinExternal")]
        public async Task<ActionResult> SignInExternal(SigninExternalUserModel model)
        {
            var token = await this.accountService.AuthenticateExternal(model.Email, model.FirstName, model.LastName, model.Occupation, model.DefaultLanguage, model.Provider, model.Token);

            if (token == null)
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
                
                if(identity.FindFirst("provider") != null && identity.FindFirst("id") != null && Guid.TryParse(identity.FindFirst("id").Value, out id))
                {
                    string providerName = identity.FindFirst("provider").Value;

                    bool result = await this.accountService.Unauthenticate(id, providerName);

                    return new JsonResult(result) { StatusCode = StatusCodes.Status200OK };
                }
            }

            return new JsonResult(true) { StatusCode = StatusCodes.Status200OK };
        }
    }
}
