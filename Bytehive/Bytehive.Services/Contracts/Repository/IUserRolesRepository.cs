using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Repository
{
    public interface IUserRolesRepository
    {
        Task<TModel> Get<TModel>(Guid roleId, Guid userId);

        Task<bool> Create<TModel>(TModel userRole);

        Task Delete(Guid roleId, Guid userId);
    }
}
