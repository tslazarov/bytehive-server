using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bytehive.Payment.Contracts;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;

namespace Bytehive.Payment.PayPal
{
    public class PaypalProvider : IPaymentProvider
    {
        private IPayPalClient paypalClient;

        public PaypalProvider(IPayPalClient paypalClient)
        {
            this.paypalClient = paypalClient;
        }

        public async Task<object> CreateOrder()
        {
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(BuildRequestBody());

            var response = await this.paypalClient.Client().Execute(request);

                var result = response.Result<Order>();

                foreach (LinkDescription link in result.Links)
                {
                }

                AmountWithBreakdown amount = result.PurchaseUnits[0].AmountWithBreakdown;

            return response.Result<Order>();
        }

        private static OrderRequest BuildRequestBody()
        {
            OrderRequest orderRequest = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",

                ApplicationContext = new ApplicationContext
                {
                    BrandName = "Bytehive",
                    LandingPage = "BILLING",
                    UserAction = "CONTINUE",
                    ShippingPreference = "NO_SHIPPING"
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
        {
          new PurchaseUnitRequest{
            ReferenceId =  "PUHF",
            Description = "Sporting Goods",
            CustomId = "CUST-HighFashions",
            SoftDescriptor = "HighFashions",
            AmountWithBreakdown = new AmountWithBreakdown
            {
              CurrencyCode = "EUR",
              Value = "30.00",
              AmountBreakdown = new AmountBreakdown
              {
                ItemTotal = new Money
                {
                  CurrencyCode = "EUR",
                  Value = "30.00"
                }
              }
            },
            Items = new List<Item>
            {
              new Item
              {
                Name = "Enterprise",
                Description = "Payment tier",
                Sku = "sku01",
                UnitAmount = new Money
                {
                  CurrencyCode = "EUR",
                  Value = "30.00"
                },
                Quantity = "1",
                Category = "PHYSICAL_GOODS"
              }
            }
          }
        }
            };

            return orderRequest;
        }

    }
}
