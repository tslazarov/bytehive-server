using Bytehive.Data.Models;
using System.Threading.Tasks;

namespace Bytehive.Payments.Contracts
{
    public interface IPaymentProvider
    {
        Task<object> CreateOrder(PaymentTier paymentTier);

        Task<object> AuthorizeOrder(string orderId);

        Task<object> GetOrder(string orderId);

        Task<object> VerifyOrder(string orderId);


    }
}
