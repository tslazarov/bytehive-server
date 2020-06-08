using AutoMapper;
using Bytehive.Data.Models;
using Bytehive.Services.Contracts.Repository;
using Bytehive.Services.Contracts.Services;
using Bytehive.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bytehive.Services
{
    public class FaqCategoriesService : IFaqCategoriesService
    {
        private readonly IFaqCategoriesRepository faqCategoriesRepository;

        public FaqCategoriesService(IFaqCategoriesRepository faqCategoriesRepository)
        {
            this.faqCategoriesRepository = faqCategoriesRepository;
        }

        public async Task<TModel> GetFaqCategory<TModel>(Guid id)
        {
            return await this.faqCategoriesRepository.Get<TModel>(id);
        }

        public async Task<IEnumerable<TModel>> GetFaqCategories<TModel>()
        {
            return await this.faqCategoriesRepository.GetAll<TModel>();
        }

        public async Task<bool> Create(FAQCategory category)
        {
            return await this.faqCategoriesRepository.Create<FAQCategory>(category);
        }

        public async Task<bool> Update(FAQCategory category)
        {
            return await this.faqCategoriesRepository.Update<FAQCategory>(category);
        }

        public async Task<bool> Delete(FAQCategory category)
        {
            return await this.faqCategoriesRepository.Delete(category.Id);
        }
    }
}
