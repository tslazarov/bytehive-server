using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Services
{
    public interface IPaymentTiersService
    {
        Task<TModel> GetPaymentTier<TModel>(Guid id);

        Task<TModel> GetPaymentTier<TModel>(string name);

        Task<IEnumerable<TModel>> GetPaymentTiers<TModel>();

        Task<bool> Create(PaymentTier paymentTier);

        Task<bool> Update(PaymentTier paymentTier);

        Task<bool> Delete(PaymentTier paymentTier);
    }
}