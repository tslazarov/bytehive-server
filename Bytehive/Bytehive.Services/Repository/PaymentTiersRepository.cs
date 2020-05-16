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
    public class PaymentTiersRepository : IPaymentTiersRepository
    { 
        private readonly BytehiveDbContext db;
        private readonly IMapper mapper;

        public PaymentTiersRepository(BytehiveDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<TModel>> GetAll<TModel>()
        {
            return await this.db.PaymentTiers
                  .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                  .ToListAsync();
        }

        public async Task<TModel> Get<TModel>(Guid id)
        {
            return await this.db.PaymentTiers
                .Where(sr => sr.Id == id)
                .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<TModel> Get<TModel>(string name)
        {
            return await this.db.PaymentTiers
                .Where(u => u.Name.ToLower() == name.ToLower())
                .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> Create<TModel>(TModel paymentTier)
        {
            if (paymentTier is PaymentTier)
            {
                this.db.PaymentTiers.Add(paymentTier as PaymentTier);

                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> Update<TModel>(TModel paymentTier)
        {
            if (paymentTier is PaymentTier)
            {
                var localPaymentTier = paymentTier as PaymentTier;
                this.db.DetachLocal(localPaymentTier, localPaymentTier.Id);
                this.db.PaymentTiers.Update(localPaymentTier);
                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> Delete(Guid id)
        {
            var paymentTier = await this.db.PaymentTiers.FindAsync(id);

            if (paymentTier != null)
            {
                this.db.PaymentTiers.Remove(paymentTier);
                await this.db.SaveChangesAsync();

                return true;

            }

            return false;
        }
    }
}
