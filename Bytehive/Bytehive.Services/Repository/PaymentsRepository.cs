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
    public class PaymentsRepository : IPaymentsRepository
    { 
        private readonly BytehiveDbContext db;
        private readonly IMapper mapper;

        public PaymentsRepository(BytehiveDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<TModel>> GetAll<TModel>()
        {
            return await this.db.Payments
                  .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                  .ToListAsync();
        }

        public async Task<TModel> Get<TModel>(Guid id)
        {
            return await this.db.Payments
                .Where(sr => sr.Id == id)
                .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> Create<TModel>(TModel payment)
        {
            if (payment is Payment)
            {
                this.db.Payments.Add(payment as Payment);

                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> Update<TModel>(TModel payment)
        {
            if (payment is Payment)
            {
                var localPayment = payment as Payment;
                this.db.DetachLocal(localPayment, localPayment.Id);
                this.db.Payments.Update(localPayment);
                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> Delete(Guid id)
        {
            var payment = await this.db.Payments.FindAsync(id);

            if (payment != null)
            {
                this.db.Payments.Remove(payment);
                await this.db.SaveChangesAsync();

                return true;

            }

            return false;
        }
    }
}
