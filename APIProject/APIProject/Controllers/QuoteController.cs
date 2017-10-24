using APIProject.GlobalVariables;
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
    [RoutePrefix("api/quote")]
    public class QuoteController : ApiController
    {
        private readonly IQuoteService _quoteService;

        public QuoteController(IQuoteService _quoteService)
        {
            this._quoteService = _quoteService;
        }

        [Route("GetOpportunityQuote")]
        public IHttpActionResult GetOpportunityQuote(int opportunityID = 0)
        {
            if (opportunityID == 0)
            {
                return BadRequest();
            }
            var foundQuote = _quoteService.GetOpportunityQuote(opportunityID);
            if(foundQuote != null)
            {
                return Ok(new QuoteViewModel(foundQuote));
            }
            else
            {
                return NotFound();
            }
        }

        [Route("PostNewQuote")]
        public IHttpActionResult PostNewQuote(PostNewQuoteViewModel request)
        {
            if(!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            var distinctIDs = new HashSet<int>(request.SalesItemIDs);
            bool allDifferent = distinctIDs.Count == request.SalesItemIDs.Count;
            if (!allDifferent)
            {
                return BadRequest(CustomError.DuplicateIDs);
            }

            try
            {
                _quoteService.CreateQuote(request.ToQuoteModel(),request.SalesItemIDs);
                return Ok();
            }catch(Exception serviceException)
            {
                return BadRequest(serviceException.Message);
            }
        }
    }
}
