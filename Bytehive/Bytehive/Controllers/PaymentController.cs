using Bytehive.Data.Models;
using Bytehive.Models.Payment;
using Bytehive.Payments.Contracts;
using Bytehive.Services.Contracts.Services;
using Bytehive.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bytehive.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : Controller
    {
        private readonly IOrdersService ordersService;
        private readonly IPaymentsService paymentsService;
        private readonly IPaymentTiersService paymentTiersService;
        private readonly IUsersService usersService;

        public PaymentController(IOrdersService ordersService,
            IPaymentsService paymentsService,
            IPaymentTiersService paymentTiersService,
            IUsersService usersService)
        {
            this.ordersService = ordersService;
            this.paymentsService = paymentsService;
            this.paymentTiersService = paymentTiersService;
            this.usersService = usersService;
        }

        [HttpGet]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("all")]
        public async Task<ActionResult> All()
        {
            var scrapeRequests = await this.paymentsService.GetPayments<PaymentListViewModel>();

            return new JsonResult(scrapeRequests.OrderByDescending(i => i.CreationDate)) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet]
        [Route("tier/all")]
        public async Task<ActionResult> TierAll()
        {
            var paymentTiers = await this.paymentTiersService.GetPaymentTiers<PaymentTierListViewModel>();

            return new JsonResult(paymentTiers.OrderBy(i => i.Price)) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("create")]
        public async Task<ActionResult> Create(CreateOrderModel model)
        {
            var paymentTier = await this.paymentTiersService.GetPaymentTier<PaymentTier>(model.Tier);

            var order = await this.ordersService.CreateOrder(model.Provider, paymentTier);

            return new JsonResult(JsonConvert.SerializeObject(order)) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("authorize")]
        public async Task<ActionResult> Authorize(AuthorizeOrderModel model)
        {
            var order = await this.ordersService.AuthorizeOrder(model.Provider, model.OrderId);

            return new JsonResult(JsonConvert.SerializeObject(order)) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost]
        [Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("verify")]
        public async Task<ActionResult> Verify(AuthorizeOrderModel model)
        {
            var payment = await this.ordersService.VerifyOrder(model.Provider, model.OrderId);

            var capture = payment as PayPalCheckoutSdk.Payments.Capture;

            if(capture.Status != "CANCELLED" && capture.Status != "FAILED")
            {
                ClaimsIdentity identity = User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    Guid id;

                    if (identity.FindFirst("id") != null && Guid.TryParse(identity.FindFirst("id").Value, out id))
                    {
                        var order = await this.ordersService.GetOrder(model.Provider, model.OrderId) as Order;
                        var orderedTier = order.PurchaseUnits.FirstOrDefault();

                        if(orderedTier != null)
                        {
                            var itemTier = orderedTier.Items.FirstOrDefault();
                            if(itemTier != null)
                            {
                                var paymentTier = await this.paymentTiersService.GetPaymentTier<PaymentTier>(itemTier.Name);

                                if(paymentTier != null)
                                {
                                    var newPayment = new Payment();
                                    newPayment.Id = Guid.NewGuid();
                                    newPayment.CreationDate = DateTime.UtcNow;
                                    newPayment.ExternalId = model.OrderId;
                                    newPayment.Provider = model.Provider;
                                    newPayment.Status = PaymentStatus.Completed;
                                    newPayment.UserId = id;
                                    newPayment.Price = paymentTier.Price;
                                    newPayment.PaymentTierId = paymentTier.Id;

                                    var newPaymentCreated = await this.paymentsService.Create(newPayment);

                                    if (newPaymentCreated)
                                    {
                                        var user = await this.usersService.GetUser<User>(id);

                                        if(user != null)
                                        {
                                            user.Tokens += paymentTier.Value;
                                        }

                                        var userUpdated = await this.usersService.Update(user);

                                        return new JsonResult(newPaymentCreated && userUpdated) { StatusCode = StatusCodes.Status200OK };
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new JsonResult(false) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpDelete]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("delete")]
        public async Task<ActionResult> Delete(string id)
        {
            Guid parsedId;
            bool deleted = false;

            if (Guid.TryParse(id, out parsedId))
            {
                var payment = await this.paymentsService.GetPayment<Payment>(parsedId);

                if (payment != null)
                {
                    deleted = await this.paymentsService.Delete(payment);
                }
            }

            var statusCode = deleted ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;

            return new JsonResult(deleted) { StatusCode = statusCode };
        }
    }
}
