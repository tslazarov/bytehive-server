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
    public class FaqRepository : IFaqRepository
    {
        private readonly BytehiveDbContext db;
        private readonly IMapper mapper;

        public FaqRepository(BytehiveDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<TModel>> GetAll<TModel>()
        {
            return await this.db.FAQs
                  .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                  .ToListAsync();
        }

        public async Task<TModel> Get<TModel>(Guid id)
        {
            return await this.db.FAQs
                .Where(sr => sr.Id == id)
                .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> Create<TModel>(TModel faq)
        {
            if (faq is FAQ)
            {
                this.db.FAQs.Add(faq as FAQ);

                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> Update<TModel>(TModel faq)
        {
            if (faq is FAQ)
            {
                var localFaq = faq as FAQ;
                this.db.DetachLocal(localFaq, localFaq.Id);
                this.db.FAQs.Update(localFaq);
                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> Delete(Guid id)
        {
            var faq = await this.db.FAQs.FindAsync(id);

            if (faq != null)
            {
                this.db.FAQs.Remove(faq);
                await this.db.SaveChangesAsync();

                return true;

            }

            return false;
        }
    }
}
