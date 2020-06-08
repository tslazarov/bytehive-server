using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Services
{
    public interface IFaqsService
    {
        Task<TModel> GetFaq<TModel>(Guid id);

        Task<IEnumerable<TModel>> GetFaqs<TModel>();

        Task<bool> Create(FAQ faq);

        Task<bool> Update(FAQ faq);

        Task<bool> Delete(FAQ faq);
    }
}