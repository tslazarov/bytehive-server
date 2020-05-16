using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Services
{
    public interface IPaymentsService
    {
        Task<TModel> GetPayment<TModel>(Guid id);

        Task<IEnumerable<TModel>> GetPayments<TModel>();

        Task<bool> Create(Payment payment);

        Task<bool> Update(Payment payment);

        Task<bool> Delete(Payment payment);
    }
}