using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Services
{
    public interface IUsersService
    {
        Task<IEnumerable<TModel>> GetUsers<TModel>();
        
        Task<User> GetUser(string email, string providerName);

        Task<bool> Create(User user);

        Task<bool> AssignRole(Guid userId, string roleName);
    }
}