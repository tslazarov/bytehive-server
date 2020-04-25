using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bytehive.Data;
using Bytehive.Data.Models;
using Bytehive.Services.Contracts;
using Bytehive.Services.Contracts.Repository;
using Bytehive.Services.Contracts.Services;
using Bytehive.Services.Dto;
using Bytehive.Services.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUsersRepository usersRepository;
        private readonly IRolesRepository rolesRepository;
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly IUserRolesRepository userRolesRepository;
        private readonly ITokenFactory tokenFactory;
        private readonly IJwtFactory jwtFactory;
        private readonly IExternalTokenValidator externalTokenValidator;

        public AccountService(IUsersRepository usersRepository,
            IRolesRepository rolesRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IUserRolesRepository userRolesRepository,
            ITokenFactory tokenFactory,
            IJwtFactory jwtFactory,
            IExternalTokenValidator externalTokenValidator)
        {
            this.usersRepository = usersRepository;
            this.rolesRepository = rolesRepository;
            this.refreshTokenRepository = refreshTokenRepository;
            this.userRolesRepository = userRolesRepository;
            this.tokenFactory = tokenFactory;
            this.jwtFactory = jwtFactory;
            this.externalTokenValidator = externalTokenValidator;
        }

        public async Task<CombinedToken> AuthenticateLocal(User user, string email, string password)
        {
            if(user == null)
            {
                user = await this.usersRepository.Get<User>(email, Constants.Strings.UserProviders.DefaultProvider);
            }

            if (user != null)
            {
                var hashedPassword = PasswordHelper.CreatePasswordHash(password, user.Salt);

                if (user.HashedPassword == hashedPassword)
                {
                    var refreshTokenValue = this.tokenFactory.GenerateToken();
                    var refreshToken = new Dto.RefreshToken(refreshTokenValue);
                    var refreshTokenDto = new Data.Models.RefreshToken(Guid.NewGuid(), refreshTokenValue, DateTime.UtcNow.AddDays(2), user.Id);

                    var refreshTokenCreated = await this.refreshTokenRepository.Create(refreshTokenDto);

                    if(refreshTokenCreated)
                    {
                        user.RefreshTokens.Add(refreshTokenDto);

                        await this.usersRepository.Update(user);
                    }

                    var roleIds = user.UserRoles.Select(u => u.RoleId);
                    var roles = (await this.rolesRepository.GetAll<Role>()).Where(r => roleIds.Contains(r.Id)).Select(r => r.Name);

                    var accessToken = await this.jwtFactory.GenerateEncodedToken(user, string.Join(", ", roles));

                    return new CombinedToken() { AccessToken = accessToken, RefreshToken = refreshToken };
                }
            }

            return null;
        }

        public async Task<CombinedToken> AuthenticateExternal(string email, string firstName, string lastName, int occupation, int defaultLanguage, string providerName, string token)
        {
            bool externalValidated = false;

            if(providerName == Constants.Strings.UserProviders.FacebookProvider)
            {
                externalValidated = await this.externalTokenValidator.ValidateFacebook(token);
            }
            else if(providerName == Constants.Strings.UserProviders.GoogleProvider)
            {
                externalValidated = await this.externalTokenValidator.ValidateGoogle(token);
            }

            if(externalValidated)
            {
                var userCreated = false;
                var roleAssigned = false;

                var user = await this.usersRepository.Get<User>(email, providerName);

                if (user == null)
                {
                    user = new User(Guid.NewGuid(), email, firstName, lastName, providerName);
                    user.Occupation = (Occupation)occupation;
                    user.DefaultLanguage = (Language)defaultLanguage;
                    user.RegistrationDate = DateTime.UtcNow;

                    userCreated = await this.usersRepository.Create(user);
                    roleAssigned = await this.AssignRole(user.Id, Constants.Strings.Roles.User);
                }

                if (user != null || (userCreated && roleAssigned))
                {
                    var refreshTokenValue = this.tokenFactory.GenerateToken();
                    var refreshToken = new Dto.RefreshToken(refreshTokenValue);
                    var refreshTokenDto = new Data.Models.RefreshToken(Guid.NewGuid(), refreshTokenValue, DateTime.UtcNow.AddDays(2), user.Id);

                    var refreshTokenCreated = await this.refreshTokenRepository.Create(refreshTokenDto);

                    if (refreshTokenCreated)
                    {
                        user.RefreshTokens.Add(refreshTokenDto);

                        await this.usersRepository.Update(user);
                    }

                    var roleIds = user.UserRoles.Select(u => u.RoleId);
                    var roles = (await this.rolesRepository.GetAll<Role>()).Where(r => roleIds.Contains(r.Id)).Select(r => r.Name);

                    var accessToken = await this.jwtFactory.GenerateEncodedToken(user, string.Join(", ", roles));

                    return new CombinedToken() { AccessToken = accessToken, RefreshToken = refreshToken };
                }
            }

            return null;
        }

        public async Task<CombinedToken> RefreshToken(string currentRefreshToken)
        {
            var token = await this.refreshTokenRepository.Get<Data.Models.RefreshToken>(currentRefreshToken);

            if (token != null && token.Active)
            {
                var user = await this.usersRepository.Get<User>(token.UserId);

                if(user != null)
                {
                    var refreshTokenValue = this.tokenFactory.GenerateToken();
                    var refreshToken = new Dto.RefreshToken(refreshTokenValue);
                    var refreshTokenDto = new Data.Models.RefreshToken(Guid.NewGuid(), refreshTokenValue, DateTime.UtcNow.AddDays(2), token.UserId);

                    var refreshTokenCreated = await this.refreshTokenRepository.Create(refreshTokenDto);

                    if (refreshTokenCreated)
                    {
                        user.RefreshTokens.Add(refreshTokenDto);

                        await this.usersRepository.Update(user);
                    }

                    var roleIds = user.UserRoles.Select(u => u.RoleId);
                    var roles = (await this.rolesRepository.GetAll<Role>()).Where(r => roleIds.Contains(r.Id)).Select(r => r.Name);

                    var accessToken = await this.jwtFactory.GenerateEncodedToken(user, string.Join(", ", roles));

                    return new CombinedToken() { AccessToken = accessToken, RefreshToken = refreshToken };
                }
            }

            return null;
        }


        public async Task<bool> Unauthenticate(Guid id, string providerName)
        {
            var user = await this.usersRepository.Get<User>(id);

            if (user != null)
            {
                foreach (var token in user.RefreshTokens)
                {
                    await this.refreshTokenRepository.Delete(token.Id);
                }

                user.RefreshTokens.Clear();
                await this.usersRepository.Update(user);
            }

            return true;
        }

        private async Task<bool> AssignRole(Guid userId, string roleName)
        {
            var role = await this.rolesRepository.Get<Role>(roleName);

            if (role != null)
            {
                var userRole = await this.userRolesRepository.Get<UserRole>(role.Id, userId);
                if(userRole == null)
                {
                    userRole = new UserRole() { RoleId = role.Id, UserId = userId };
                    return await this.userRolesRepository.Create(userRole);
                }

                return true;
            }

            return false;
        }
    }
}
