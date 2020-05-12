using Bytehive.Models.Payment;
using Bytehive.Payment;
using Bytehive.Payment.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        [HttpPost]
        //[Authorize(Policy = Constants.Strings.Roles.User)]
        [Route("create")]
        public async Task<ActionResult> Create(CreatePaymentModel model)
        {
            var order = await this.paymentService.CreateOrder(model.Provider);

            return new JsonResult(JsonConvert.SerializeObject(order)) { StatusCode = StatusCodes.Status200OK };
        }
    }
}
