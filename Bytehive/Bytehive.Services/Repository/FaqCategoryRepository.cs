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
    public class FaqCategoryRepository : IFaqCategoryRepository
    {
        private readonly BytehiveDbContext db;
        private readonly IMapper mapper;

        public FaqCategoryRepository(BytehiveDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<TModel>> GetAll<TModel>()
        {
            return await this.db.FAQCategories
                  .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                  .ToListAsync();
        }

        public async Task<TModel> Get<TModel>(Guid id)
        {
            return await this.db.FAQCategories
                .Where(sr => sr.Id == id)
                .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> Create<TModel>(TModel category)
        {
            if (category is FAQCategory)
            {
                this.db.FAQCategories.Add(category as FAQCategory);

                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> Update<TModel>(TModel category)
        {
            if (category is FAQCategory)
            {
                var localCategory = category as FAQCategory;
                this.db.DetachLocal(localCategory, localCategory.Id);
                this.db.FAQCategories.Update(localCategory);
                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> Delete(Guid id)
        {
            var category = await this.db.FAQCategories.FindAsync(id);

            if (category != null)
            {
                this.db.FAQCategories.Remove(category);
                await this.db.SaveChangesAsync();

                return true;

            }

            return false;
        }
    }
}
