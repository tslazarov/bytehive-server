using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bytehive.Payment.Contracts;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;
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
            OrdersCreateRequest request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(BuildRequestBody());

            var response = await this.paypalClient.Client().Execute(request);

            var result = response.Result<Order>();

            return result;
        }

        public async Task<object> AuthorizeOrder(string orderId)
        {
            OrdersAuthorizeRequest request = new OrdersAuthorizeRequest(orderId);
            request.Prefer("return=representation");
            request.RequestBody(new AuthorizeRequest());

            var response = await this.paypalClient.Client().Execute(request);

            var result = response.Result<Order>();

            return result;
        }

        public async Task<object> GetOrder(string orderId)
        {
            OrdersGetRequest request = new OrdersGetRequest(orderId);
            var response = await this.paypalClient.Client().Execute(request);

            var result = response.Result<Order>();

            return result;
        }

        public async Task<object> VerifyOrder(string orderId)
        {
            OrdersGetRequest request = new OrdersGetRequest(orderId);
            var response = await this.paypalClient.Client().Execute(request);

            var result = response.Result<Order>();

            if(result.Status == "COMPLETED")
            {
                var purchase = result.PurchaseUnits.FirstOrDefault();

                if(purchase != null && purchase.Payments != null && purchase.Payments.Authorizations != null)
                {
                    var authorization = purchase.Payments.Authorizations.FirstOrDefault();

                    if(authorization != null)
                    {
                        var authorizationId = authorization.Id;

                        var captureResponse = await this.CapturePayment(authorizationId);

                        return captureResponse;
                    }
                }
            }

            return null;
        }

        private async Task<object> CapturePayment(string authorizationId)
        {
            AuthorizationsCaptureRequest request = new AuthorizationsCaptureRequest(authorizationId);
            request.Prefer("return=representation");
            request.RequestBody(new CaptureRequest());

            var response = await this.paypalClient.Client().Execute(request);

            var result = response.Result<PayPalCheckoutSdk.Payments.Capture>();

            return result;
        }

        private static OrderRequest BuildRequestBody()
        {
            OrderRequest orderRequest = new OrderRequest()
            {
                CheckoutPaymentIntent = "AUTHORIZE",

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
              Value = "10.00",
              AmountBreakdown = new AmountBreakdown
              {
                ItemTotal = new PayPalCheckoutSdk.Orders.Money
                {
                  CurrencyCode = "EUR",
                  Value = "10.00"
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
                UnitAmount = new PayPalCheckoutSdk.Orders.Money
                {
                  CurrencyCode = "EUR",
                  Value = "10.00"
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
