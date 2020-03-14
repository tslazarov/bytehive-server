using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts
{
    public interface IUsersService
    {
        Task<IEnumerable<TModel>> GetAll<TModel>();
 
        Task<TModel> Get<TModel>(Guid userId);

        Task<TModel> Get<TModel>(string email);

        Task Remove(Guid id);
    }
}
