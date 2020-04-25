using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Services
{
    public interface IUsersService
    {
        Task<TModel> GetUser<TModel>(Guid id);

        Task<TModel> GetUser<TModel>(string email, string providerName);

        Task<IEnumerable<TModel>> GetUsers<TModel>();

        Task<bool> Create(User user);

        Task<bool> Update(User user);

        Task<bool> Delete(User user);

        Task<bool> AssignRole(Guid userId, string roleName);
    }
}