using APIProject.Service;
using APIProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace APIProject.Controllers
{
    [RoutePrefix("api/marketingresult")]
    public class MarketingResultController : ApiController
    {
        private readonly IMarketingResultService _marketingResultService;

        public MarketingResultController(IMarketingResultService _marketingResultService)
        {
            this._marketingResultService = _marketingResultService;
        }

        [Route("GetMarketingResults")]
        //[ResponseType(MarketingResultViewModel)]
        public async Task<IHttpActionResult> GetMarketingResults(int? planID = null)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var list = _marketingResultService.GetMarketingResults();

                //check if specific plan
                if (planID.HasValue)
                {
                    list = list.Where(c => c.ID == planID);
                }

                var resultList = list.Select(c => new MarketingResultViewModel(c));
                return resultList;
            });
            return Ok(await task);
        }

        [Route("PostMarketingResults")]
        public IHttpActionResult PostMarketingResults(PostMarketingResultsViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _marketingResultService.CreateResults(request.ToMarketingResultModels(), request.IsFinished);
            return Ok();
        }
    }
}
