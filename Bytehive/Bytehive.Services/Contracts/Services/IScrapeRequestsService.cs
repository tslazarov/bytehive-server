using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Services
{
    public interface IScrapeRequestsService
    {
        Task<ScrapeRequest> GetScrapeRequest(Guid id);

        Task<IEnumerable<TModel>> GetScrapeRequests<TModel>();

        Task<bool> Create(ScrapeRequest ScrapeRequest);

        Task<bool> Update(ScrapeRequest ScrapeRequest);

        Task<bool> Delete(ScrapeRequest ScrapeRequest);
    }
}