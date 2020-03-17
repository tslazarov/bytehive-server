using AutoMapper;
using Bytehive.Data.Models;
using Bytehive.Services.Contracts.Repository;
using Bytehive.Services.Contracts.Services;
using System.Threading.Tasks;

namespace Bytehive.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository usersRepository;

        public UsersService(IUsersRepository usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        public async Task<User> GetUser(string email)
        {
            return await this.usersRepository.Get<User>(email);
        }

        public async Task<bool> Create(User user)
        {
            return await this.usersRepository.Create<User>(user);
        }
    }
}
