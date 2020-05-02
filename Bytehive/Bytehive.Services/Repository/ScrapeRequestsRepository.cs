using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bytehive.Data;
using Bytehive.Data.Models;
using Bytehive.Services.Contracts;
using Bytehive.Services.Contracts.Repository;
using Bytehive.Services.Contracts.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Services.Repository
{
    public class ScrapeRequestsRepository : IScrapeRequestsRepository
    {
        private readonly BytehiveDbContext db;
        private readonly IMapper mapper;

        public ScrapeRequestsRepository(BytehiveDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<TModel>> GetAll<TModel>()
        {
            return await this.db.ScrapeRequests
                  .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                  .ToListAsync();
        }

        public async Task<TModel> Get<TModel>(Guid id)
        {
            return await this.db.ScrapeRequests
                .Where(sr => sr.Id == id)
                .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> Create<TModel>(TModel scrapeRequest)
        {
            if (scrapeRequest is ScrapeRequest)
            {
                this.db.ScrapeRequests.Add(scrapeRequest as ScrapeRequest);

                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> Update<TModel>(TModel scrapeRequest)
        {
            if (scrapeRequest is ScrapeRequest)
            {
                var localScrapeRequest = scrapeRequest as ScrapeRequest;
                this.db.DetachLocal(localScrapeRequest, localScrapeRequest.Id);
                this.db.ScrapeRequests.Update(localScrapeRequest);
                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> Delete(Guid id)
        {
            var scrapeRequest = await this.db.ScrapeRequests.FindAsync(id);

            if (scrapeRequest != null)
            {
                this.db.ScrapeRequests.Remove(scrapeRequest);
                await this.db.SaveChangesAsync();

                return true;

            }

            return false;
        }
    }
}
