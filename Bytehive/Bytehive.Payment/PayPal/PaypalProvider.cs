using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bytehive.Data.Models;
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

        public async Task<object> CreateOrder(PaymentTier paymentTier)
        {
            OrdersCreateRequest request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(BuildRequestBody(paymentTier));

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

        private static OrderRequest BuildRequestBody(PaymentTier paymentTier)
        {
            OrderRequest orderRequest = new OrderRequest();
            orderRequest.CheckoutPaymentIntent = "AUTHORIZE";

            orderRequest.ApplicationContext = new ApplicationContext();
            orderRequest.ApplicationContext.BrandName = "Bytehive";
            orderRequest.ApplicationContext.LandingPage = "BILLING";
            orderRequest.ApplicationContext.UserAction = "CONTINUE";
            orderRequest.ApplicationContext.ShippingPreference = "NO_SHIPPING";

            orderRequest.PurchaseUnits = new List<PurchaseUnitRequest>();

            var purchaseUnitRequest = new PurchaseUnitRequest();
            purchaseUnitRequest.ReferenceId = "PUHF";
            purchaseUnitRequest.Description = "Bytehive Tier";
            purchaseUnitRequest.CustomId = string.Format("CUST-{0}", paymentTier.Name.ToUpper());
            purchaseUnitRequest.SoftDescriptor = paymentTier.Name;

            purchaseUnitRequest.AmountWithBreakdown = new AmountWithBreakdown();
            purchaseUnitRequest.AmountWithBreakdown.CurrencyCode = "EUR";
            purchaseUnitRequest.AmountWithBreakdown.Value = paymentTier.Price.ToString();

            purchaseUnitRequest.AmountWithBreakdown.AmountBreakdown = new AmountBreakdown();
            purchaseUnitRequest.AmountWithBreakdown.AmountBreakdown.ItemTotal = new PayPalCheckoutSdk.Orders.Money();
            purchaseUnitRequest.AmountWithBreakdown.AmountBreakdown.ItemTotal.CurrencyCode = "EUR";
            purchaseUnitRequest.AmountWithBreakdown.AmountBreakdown.ItemTotal.Value = paymentTier.Price.ToString();

            purchaseUnitRequest.Items = new List<Item>();

            var item = new Item();
            item.Name = paymentTier.Name;
            item.Sku = paymentTier.Sku;
            item.UnitAmount = new PayPalCheckoutSdk.Orders.Money();
            item.UnitAmount.CurrencyCode = "EUR";
            item.UnitAmount.Value = paymentTier.Price.ToString();
            item.Quantity = "1";
            item.Category = "DIGITAL_GOODS";

            purchaseUnitRequest.Items.Add(item);

            orderRequest.PurchaseUnits.Add(purchaseUnitRequest);

            return orderRequest;
        }
    }
}
