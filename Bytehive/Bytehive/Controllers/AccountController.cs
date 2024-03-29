﻿using AutoMapper;
using Bytehive.Data.Models;
using Bytehive.Models.Account;
using Bytehive.Models.Users;
using Bytehive.Notifications;
using Bytehive.Services.Contracts.Services;
using Bytehive.Services.Utilities;
using Bytehive.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
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
        private readonly IAzureBlobStorageProvider azureBlobStorageProvider;

        public AccountController(IUsersService usersService,
            IAccountService accountService,
            ISendGridSender notificationsSender,
            IMapper mapper,
            IAzureBlobStorageProvider azureBlobStorageProvider)
        {
            this.usersService = usersService;
            this.accountService = accountService;
            this.mapper = mapper;
            this.notificationsSender = notificationsSender;
            this.azureBlobStorageProvider = azureBlobStorageProvider;
        }

        [HttpGet]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("profile")]
        public async Task<ActionResult> Profile()
        {
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                Guid id;

                if (identity.FindFirst("id") != null && Guid.TryParse(identity.FindFirst("id").Value, out id))
                {
                    var user = await this.usersService.GetUser<UserDetailViewModel>(id);

                    if (user != null)
                    {
                        return new JsonResult(user) { StatusCode = StatusCodes.Status200OK };
                    }
                }
            }

            return new JsonResult(null) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("signup")]
        public async Task<ActionResult> SignUp(SignupUserModel model)
        {
            var existingUser = await this.usersService.GetUser<User>(model.Email, Constants.Strings.UserProviders.DefaultProvider);

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
            var user = await this.usersService.GetUser<User>(model.Email, Constants.Strings.UserProviders.DefaultProvider);

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
            var user = await this.usersService.GetUser<User>(model.Email, Constants.Strings.UserProviders.DefaultProvider);

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

        [HttpPut]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("changepassword")]
        public async Task<ActionResult> ChangePassword(ChangePasswordModel model)
        {
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                Guid id;

                if (identity.FindFirst("id") != null && Guid.TryParse(identity.FindFirst("id").Value, out id))
                {
                    var user = await this.usersService.GetUser<User>(id);

                    if (user != null)
                    {
                        var hashedPassword = PasswordHelper.CreatePasswordHash(model.CurrentPassword, user.Salt);

                        if (user.HashedPassword == hashedPassword && model.Password == model.ConfirmPassword)
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
                }
            }

            return new JsonResult("Password was not changed") { StatusCode = StatusCodes.Status400BadRequest };
        }

        [HttpPut]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("settings")]
        public async Task<ActionResult> ChangeSettings(ChangeSettingsModel model)
        {
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                Guid id;

                if (identity.FindFirst("id") != null && Guid.TryParse(identity.FindFirst("id").Value, out id))
                {
                    var user = await this.usersService.GetUser<User>(id);

                    if (user != null)
                    {
                        user.DefaultLanguage = (Language)model.DefaultLanguage;

                        var userUpdated = await this.usersService.Update(user);

                        if (userUpdated)
                        {
                            return new JsonResult("Settings were successfully changed") { StatusCode = StatusCodes.Status200OK };
                        }
                    }
                }
            }

            return new JsonResult("Settings were not changed") { StatusCode = StatusCodes.Status400BadRequest };
        }

        [HttpPut]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("information")]
        public async Task<ActionResult> ChangeInformation(ChangeInformationModel model)
        {
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                Guid id;

                if (identity.FindFirst("id") != null && Guid.TryParse(identity.FindFirst("id").Value, out id))
                {
                    var user = await this.usersService.GetUser<User>(id);

                    if (user != null)
                    {
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        user.Occupation = (Occupation)model.Occupation;

                        var userUpdated = await this.usersService.Update(user);

                        if (userUpdated)
                        {
                            return new JsonResult("Profile information was successfully changed") { StatusCode = StatusCodes.Status200OK };
                        }
                    }
                }
            }

            return new JsonResult("Profile information was not changed") { StatusCode = StatusCodes.Status400BadRequest };
        }

        [HttpPut]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("email")]
        public async Task<ActionResult> ChangeEmail(ChangeEmailModel model)
        {
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                Guid id;

                if (identity.FindFirst("id") != null && Guid.TryParse(identity.FindFirst("id").Value, out id))
                {
                    var user = await this.usersService.GetUser<User>(id);

                    if (user != null)
                    {
                        var existingUser = await this.usersService.GetUser<User>(model.Email, Constants.Strings.UserProviders.DefaultProvider);

                        if(existingUser != null)
                        {
                            return new JsonResult("A user with the corresponding email already exists.") { StatusCode = StatusCodes.Status400BadRequest };
                        }

                        var hashedPassword = PasswordHelper.CreatePasswordHash(model.Password, user.Salt);

                        if (user.HashedPassword == hashedPassword)
                        {
                            user.Email = model.Email;

                            var userUpdated = await this.usersService.Update(user);

                            if (userUpdated)
                            {
                                return new JsonResult("Email was successfully changed.") { StatusCode = StatusCodes.Status200OK };
                            }
                        }
                        else
                        {
                            return new JsonResult("The provided password is incorrect.") { StatusCode = StatusCodes.Status400BadRequest };
                        }
                    }
                }
            }

            return new JsonResult("Email information was not changed.") { StatusCode = StatusCodes.Status400BadRequest };
        }

        [HttpGet]
        [Route("image/{imagePath}")]
        public async Task<ActionResult> GetAvatar(string imagePath)
        {

            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    var file = await this.azureBlobStorageProvider.DownloadBlob(AzureBlobStorageProvider.ImagesContainerName, imagePath);
                    return File(file.Content, file.ContentType);
                }
                catch
                {
                    return new JsonResult("File not found") { StatusCode = StatusCodes.Status404NotFound };
                }
            }

            return new JsonResult("File not found") { StatusCode = StatusCodes.Status404NotFound };
        }

        [HttpPut]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("image")]
        public async Task<ActionResult> ChangeImage(ChangeImageModel model)
        {
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                Guid id;

                if (identity.FindFirst("id") != null && Guid.TryParse(identity.FindFirst("id").Value, out id))
                {
                    var user = await this.usersService.GetUser<User>(id);

                    if (user != null)
                    {
                        string imageBase64 = Regex.Replace(model.ImageBase64, "^data:image/[a-zA-Z]+;base64,", string.Empty);

                        var match = Regex.Match(model.ImageBase64, "^data:image/([a-zA-Z]+);base64,");
                        var extension = match.Groups.Count > 1 ? match.Groups[1].ToString() : ".png";

                        var fileName = string.Format("{0}-{1}.{2}", user.Id, DateTime.UtcNow.Ticks, extension);
                        byte[] bytes = Convert.FromBase64String(imageBase64);
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            var blobContent = await this.azureBlobStorageProvider.UploadBlob(AzureBlobStorageProvider.ImagesContainerName, fileName, string.Format(".{0}", extension), ms);

                            if(blobContent != null)
                            {
                                var oldImageDeleted = await this.azureBlobStorageProvider.DeleteBlob(AzureBlobStorageProvider.ImagesContainerName, user.Image);

                                user.Image = fileName;

                                var updated = await this.usersService.Update(user);

                                return new JsonResult(updated) { StatusCode = StatusCodes.Status200OK };
                            }
                        }
                    }
                }
            }

            return new JsonResult(false) { StatusCode = StatusCodes.Status400BadRequest };
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

    }
}
