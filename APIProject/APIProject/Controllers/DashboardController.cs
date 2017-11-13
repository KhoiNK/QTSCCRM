using APIProject.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace APIProject.Controllers
{
    [RoutePrefix("api/dashbboard")]
    public class DashboardController : ApiController
    {
        private readonly IOpportunityService _opportunityService;
        private readonly ICustomerService _customerService;
        public DashboardController(ICustomerService _customerService,
            IOpportunityService _opportunityService)
        {
            this._opportunityService = _opportunityService;
            this._customerService = _customerService;

        }

        [Route("GetDashboard")]
        public IHttpActionResult GetDashboard(int monthRange = 12,
            int month = 0)
        {
            month = (month == 0) ? DateTime.Now.Month : month;

            int considerOppCount = _opportunityService.CountConsider();
            int makeQuoteOppCount = _opportunityService.CountMakeQuote();
            int validateOppCount = _opportunityService.CountValidateQuote();
            int sendQuoteOppCount = _opportunityService.CountSendQuote();
            int wonOppCount = _opportunityService.CountWon();
            int lostOppCount = _opportunityService.CountLost();

            double ConsiderDuration = _opportunityService.AverageConsider();

            int customerCount = _customerService.GetOfficial().Count();
            int leadCount = _customerService.GetLead().Count();
        }
    }
}
