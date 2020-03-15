using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bytehive.Data;
using Bytehive.Data.Models;
using Bytehive.Services.Contracts;
using Bytehive.Services.Contracts.Repository;
using Bytehive.Services.Contracts.Services;
using Bytehive.Services.Dto;
using Bytehive.Services.Utilities;
using System.Threading.Tasks;

namespace Bytehive.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUsersRepository usersRepository;
        private readonly ITokenFactory tokenFactory;
        private readonly IJwtFactory jwtFactory;
        private readonly IMapper mapper;

        public AccountService(IUsersRepository usersRepository,
            ITokenFactory tokenFactory,
            IJwtFactory jwtFactory)
        {
            this.usersRepository = usersRepository;
            this.tokenFactory = tokenFactory;
            this.jwtFactory = jwtFactory;
        }

        public async Task<CombinedToken> Authenticate(string email, string password, string remoteIpAddress)
        {
            var user = await this.usersRepository.Get<User>(email);

            if(user != null)
            {
                var hashedPassword = PasswordHelper.CreatePasswordHash(password, user.Salt);

                if(user.HashedPassword == hashedPassword)
                {
                    var refreshTokenValue = this.tokenFactory.GenerateToken();
                    var refreshToken = new Dto.RefreshToken(refreshTokenValue);

                    user.AddRereshToken(refreshTokenValue, user.Id, remoteIpAddress, 7);
                    await this.usersRepository.Update<User>(user);


                    var accessToken = await this.jwtFactory.GenerateEncodedToken(user.Id.ToString(), user.Email);

                    return new CombinedToken() { AccessToken = accessToken, RefreshToken = refreshToken };
                }
            }

            return null;
        }
    }
}
