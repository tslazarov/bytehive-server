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
    public class PaymentsService : IPaymentsService
    {
        private readonly IPaymentsRepository paymentsRepository;

        public PaymentsService(IPaymentsRepository paymentsRepository)
        {
            this.paymentsRepository = paymentsRepository;
        }

        public async Task<TModel> GetPayment<TModel>(Guid id)
        {
            return await this.paymentsRepository.Get<TModel>(id);
        }

        public async Task<IEnumerable<TModel>> GetPayments<TModel>()
        {
            return await this.paymentsRepository.GetAll<TModel>();
        }

        public async Task<bool> Create(Payment payment)
        {
            return await this.paymentsRepository.Create<Payment>(payment);
        }

        public async Task<bool> Update(Payment payment)
        {
            return await this.paymentsRepository.Update<Payment>(payment);
        }

        public async Task<bool> Delete(Payment payment)
        {
            return await this.paymentsRepository.Delete(payment.Id);
        }
    }
}
