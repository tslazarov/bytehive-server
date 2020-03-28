using Bytehive.Data.Models;
using System;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Services
{
    public interface IUsersService
    {
        Task<User> GetUser(string email, string providerName);

        Task<bool> Create(User user);

        Task<bool> AssignRole(Guid userId, string roleName);
    }
}