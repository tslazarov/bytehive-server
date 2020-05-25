using Bytehive.Data.Models;
using Bytehive.Models.Statistics;
using Bytehive.Services.Contracts.Services;
using Bytehive.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Controllers
{
    [ApiController]
    [Route("api/statistics")]
    public class StatisticsController : Controller
    {
        private readonly IScrapeRequestsService scrapeRequestsService;
        private readonly IUsersService usersService;
        private readonly IPaymentsService paymentsService;

        public StatisticsController(IScrapeRequestsService scrapeRequestsService,
            IUsersService usersService,
            IPaymentsService paymentsService)
        {
            this.scrapeRequestsService = scrapeRequestsService;
            this.usersService = usersService;
            this.paymentsService = paymentsService;
        }

        [HttpGet]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("summary")]
        public async Task<ActionResult> Summary()
        {
            var summaryViewModel = new SummaryViewModel();

            var users = await this.usersService.GetUsers<User>();
            var requests = await this.scrapeRequestsService.GetScrapeRequests<ScrapeRequest>();
            var payments = await this.paymentsService.GetPayments<Payment>();

            summaryViewModel.UsersCount = users.Count();
            summaryViewModel.RequestsCount = requests.Count();
            summaryViewModel.EntriesCount = requests.Sum(r => r.Entries);
            summaryViewModel.PaymentsTotal = payments.Sum(p => p.Price);

            return new JsonResult(summaryViewModel) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("users")]
        public async Task<ActionResult> UsersSummary()
        {
            var users = await this.usersService.GetUsers<UsersSummaryViewModel>();

            return new JsonResult(users.OrderByDescending(u => u.RegistrationDate)) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet]
        [Authorize(Policy = Constants.Strings.Roles.Administrator)]
        [Route("requests")]
        public async Task<ActionResult> RequestsSummary()
        {
            var requests = await this.scrapeRequestsService.GetScrapeRequests<ScrapeRequestsSummaryViewModel>();

            return new JsonResult(requests.OrderByDescending(u => u.CreationDate)) { StatusCode = StatusCodes.Status200OK };
        }
    }
}
