using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Repository
{
    public interface IFaqCategoriesRepository
    {
        Task<IEnumerable<TModel>> GetAll<TModel>();
 
        Task<TModel> Get<TModel>(Guid id);

        Task<bool> Create<TModel>(TModel category);

        Task<bool> Update<TModel>(TModel category);

        Task<bool> Delete(Guid id);
    }
}
