using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Repository
{
    public interface IFilesRepository
    {
        Task<IEnumerable<TModel>> GetAll<TModel>();
 
        Task<TModel> Get<TModel>(Guid id);

        Task<bool> Create<TModel>(TModel file);

        Task<bool> Update<TModel>(TModel file);

        Task<bool> Delete(Guid id);
    }
}
