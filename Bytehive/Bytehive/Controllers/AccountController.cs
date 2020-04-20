using AutoMapper;
using Bytehive.Data.Models;
using Bytehive.Models.Account;
using Bytehive.Notifications;
using Bytehive.Services.Contracts.Services;
using Bytehive.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bytehive.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly IUsersService usersService;
        private readonly IAccountService accountService;
        private readonly IMapper mapper;
        private readonly ISendGridSender notificationsSender;

        public AccountController(IUsersService usersService,
            IAccountService accountService,
            ISendGridSender notificationsSender,
            IMapper mapper)
        {
            this.usersService = usersService;
            this.accountService = accountService;
            this.mapper = mapper;
            this.notificationsSender = notificationsSender;
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

        [HttpPut]
        [AllowAnonymous]
        [Route("resetcode")]
        public async Task<ActionResult> ResetCode(ResetCodeModel model)
        {
            var user = await this.usersService.GetUser(model.Email, Constants.Strings.UserProviders.DefaultProvider);

            if(user != null)
            {
                string randomString = Guid.NewGuid().ToString().Replace("-", "").Substring(12, 8);
                string language = user.DefaultLanguage.ToString();

                string plainText = this.notificationsSender.GetResetPasswordPlainText(randomString, language);
                string htmlText = this.notificationsSender.GetResetPasswordHtml(randomString, language);

                HttpStatusCode status = await this.notificationsSender.SendMessage("support@bytehive.com", "Bytehive Support", model.Email, "[Bytehive] Password Reset", plainText, htmlText);

                if (status == HttpStatusCode.Accepted)
                {
                    user.ResetCode = randomString;

                    await this.usersService.Update(user);
                }
            }

            return new JsonResult(true) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPut]
        [AllowAnonymous]
        [Route("resetpassword")]
        public async Task<ActionResult> ResetPassword(ResetPasswordModel model)
        {
            var user = await this.usersService.GetUser(model.Email, Constants.Strings.UserProviders.DefaultProvider);

            if (user != null)
            {
                if (user.ResetCode == model.Code && model.Password == model.ConfirmPassword)
                {
                    string salt = PasswordHelper.CreateSalt(10);
                    string hashedNewPassword = PasswordHelper.CreatePasswordHash(model.Password, salt);

                    user.Salt = salt;
                    user.HashedPassword = hashedNewPassword;
                    user.ResetCode = null;

                    var userUpdated = await this.usersService.Update(user);

                    if (userUpdated)
                    {
                        return new JsonResult("Password was successfully changed") { StatusCode = StatusCodes.Status200OK };
                    }
                }
            }

            return new JsonResult("Reset code is invalid") { StatusCode = StatusCodes.Status400BadRequest };
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("refreshtoken")]
        public async Task<ActionResult> RefreshToken(RefreshToken model)
        {
            var token = await this.accountService.RefreshToken(model.Token);

            if (token == null)
            {
                return new JsonResult("Refresh token has expired") { StatusCode = StatusCodes.Status400BadRequest };
            }

            return new JsonResult(token) { StatusCode = StatusCodes.Status200OK };
        }

    }
}
