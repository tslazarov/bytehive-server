using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Services
{
    public interface IFaqCategoriesService
    {
        Task<TModel> GetFaqCategory<TModel>(Guid id);

        Task<IEnumerable<TModel>> GetFaqCategories<TModel>();

        Task<bool> Create(FAQCategory category);

        Task<bool> Update(FAQCategory category);

        Task<bool> Delete(FAQCategory category);
    }
}