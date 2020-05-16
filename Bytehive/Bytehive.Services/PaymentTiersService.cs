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
    public class PaymentTiersService : IPaymentTiersService
    {
        private readonly IPaymentTiersRepository paymentTiersRepository;

        public PaymentTiersService(IPaymentTiersRepository paymentTiersRepository)
        {
            this.paymentTiersRepository = paymentTiersRepository;
        }

        public async Task<TModel> GetPaymentTier<TModel>(Guid id)
        {
            return await this.paymentTiersRepository.Get<TModel>(id);
        }

        public async Task<TModel> GetPaymentTier<TModel>(string name)
        {
            return await this.paymentTiersRepository.Get<TModel>(name);
        }

        public async Task<IEnumerable<TModel>> GetPaymentTiers<TModel>()
        {
            return await this.paymentTiersRepository.GetAll<TModel>();
        }

        public async Task<bool> Create(PaymentTier paymentTier)
        {
            return await this.paymentTiersRepository.Create<PaymentTier>(paymentTier);
        }

        public async Task<bool> Update(PaymentTier paymentTier)
        {
            return await this.paymentTiersRepository.Update<PaymentTier>(paymentTier);
        }

        public async Task<bool> Delete(PaymentTier paymentTier)
        {
            return await this.paymentTiersRepository.Delete(paymentTier.Id);
        }
    }
}
