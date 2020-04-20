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
    public class ScrapeRequestsService : IScrapeRequestsService
    {
        private readonly IScrapeRequestsRepository scrapeRequestsRepository;

        public ScrapeRequestsService(IScrapeRequestsRepository scrapeRequestsRepository)
        {
            this.scrapeRequestsRepository = scrapeRequestsRepository;
        }

        public async Task<IEnumerable<TModel>> GetScrapeRequests<TModel>()
        {
            return await this.scrapeRequestsRepository.GetAll<TModel>();
        }

        public async Task<ScrapeRequest> GetScrapeRequest(Guid id)
        {
            return await this.scrapeRequestsRepository.Get<ScrapeRequest>(id);
        }

        public async Task<IEnumerable<TModel>> GetUsers<TModel>()
        {
            return await this.scrapeRequestsRepository.GetAll<TModel>();
        }

        public async Task<bool> Create(ScrapeRequest scrapeRequest)
        {
            return await this.scrapeRequestsRepository.Create<ScrapeRequest>(scrapeRequest);
        }

        public async Task<bool> Update(ScrapeRequest scrapeRequest)
        {
            return await this.scrapeRequestsRepository.Update<ScrapeRequest>(scrapeRequest);
        }

        public async Task<bool> Delete(ScrapeRequest scrapeRequest)
        {
            return await this.scrapeRequestsRepository.Delete(scrapeRequest.Id);
        }
    }
}
