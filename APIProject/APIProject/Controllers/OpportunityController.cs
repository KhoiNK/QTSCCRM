using APIProject.Service;
using APIProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace APIProject.Controllers
{
    [RoutePrefix("api/opportunity")]
    public class OpportunityController : ApiController
    {
        private readonly IOpportunityService _opportunityService;

        public OpportunityController(IOpportunityService _opportunityService)
        {
            this._opportunityService = _opportunityService;
        }

        [Route("GetOpportunities")]
        public IHttpActionResult GetOpportunities()
        {
            return Ok(_opportunityService.GetAllOpportunities().Select(c => new OpportunityViewModel(c)));
        }
    }
}
