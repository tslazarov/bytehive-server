using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Repository
{
    public interface IUsersRepository
    {
        Task<IEnumerable<TModel>> GetAll<TModel>();

        Task<IEnumerable<TModel>> GetAll<TModel>(string providerName);
 
        Task<TModel> Get<TModel>(Guid userId);

        Task<TModel> Get<TModel>(string email, string providerName);

        Task<bool> Create<TModel>(TModel user);

        Task<bool> Update<TModel>(TModel user);

        Task Remove(Guid id);
    }
}
