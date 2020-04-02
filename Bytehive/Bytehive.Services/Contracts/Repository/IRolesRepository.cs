using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Repository
{
    public interface IRolesRepository
    {
        Task<IEnumerable<TModel>> GetAll<TModel>();
 
        Task<TModel> Get<TModel>(Guid id);

        Task<TModel> Get<TModel>(string name);

        Task<bool> Create<TModel>(TModel role);

        Task<bool> Update<TModel>(TModel role);

        Task Delete(Guid id);
    }
}
