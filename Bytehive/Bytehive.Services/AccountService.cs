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
        private readonly ITokenFactory tokenFactory;
        private readonly IJwtFactory jwtFactory;
        private readonly IMapper mapper;

        public AccountService(IUsersRepository usersRepository,
            IRolesRepository rolesRepository,
            ITokenFactory tokenFactory,
            IJwtFactory jwtFactory)
        {
            this.usersRepository = usersRepository;
            this.rolesRepository = rolesRepository;
            this.tokenFactory = tokenFactory;
            this.jwtFactory = jwtFactory;
        }

        public async Task<CombinedToken> Authenticate(string email, string password, string remoteIpAddress)
        {
            var user = await this.usersRepository.Get<User>(email);

            if(user != null)
            {
                var hashedPassword = PasswordHelper.CreatePasswordHash(password, user.Salt);

                if (user.HashedPassword == hashedPassword)
                {
                    var refreshTokenValue = this.tokenFactory.GenerateToken();
                    var refreshToken = new Dto.RefreshToken(refreshTokenValue);

                    user.RefreshTokens.Add(new Data.Models.RefreshToken(Guid.NewGuid(), refreshTokenValue, DateTime.UtcNow.AddDays(2), user.Id, remoteIpAddress));
                        
                    await this.usersRepository.Update(user);

                    var roleIds = user.UserRoles.Select(u => u.RoleId);
                    var roles = (await this.rolesRepository.GetAll<Role>()).Where(r => roleIds.Contains(r.Id)).Select(r => r.Name);

                    var accessToken = await this.jwtFactory.GenerateEncodedToken(user, string.Join(", ", roles));

                    return new CombinedToken() { AccessToken = accessToken, RefreshToken = refreshToken };
                }
            }

            return null;
        }


        public async Task<bool> Unauthenticate(Guid id)
        {
            var user = await this.usersRepository.Get<User>(id);

            if (user != null)
            {
                user.RefreshTokens.Clear();
                await this.usersRepository.Update(user);
            }

            return true;
        }
    }
}
