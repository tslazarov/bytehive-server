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
    public class FaqsService : IFaqsService
    {
        private readonly IFaqsRepository faqsRepository;

        public FaqsService(IFaqsRepository faqsRepository)
        {
            this.faqsRepository = faqsRepository;
        }

        public async Task<TModel> GetFaq<TModel>(Guid id)
        {
            return await this.faqsRepository.Get<TModel>(id);
        }

        public async Task<IEnumerable<TModel>> GetFaqs<TModel>()
        {
            return await this.faqsRepository.GetAll<TModel>();
        }

        public async Task<bool> Create(FAQ faq)
        {
            return await this.faqsRepository.Create<FAQ>(faq);
        }

        public async Task<bool> Update(FAQ faq)
        {
            return await this.faqsRepository.Update<FAQ>(faq);
        }

        public async Task<bool> Delete(FAQ faq)
        {
            return await this.faqsRepository.Delete(faq.Id);
        }
    }
}
