using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Services
{
    public interface IScrapeRequestsService
    {
        Task<TModel> GetScrapeRequest<TModel>(Guid id);

        Task<IEnumerable<TModel>> GetScrapeRequests<TModel>();

        Task<bool> Create(ScrapeRequest scrapeRequest);

        Task<bool> Update(ScrapeRequest scrapeRequest);

        Task<bool> Delete(ScrapeRequest scrapeRequest);
    }
}