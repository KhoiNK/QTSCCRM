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
        private readonly IMarketingPlanService _marketingPlanService;

        public MarketingResultController(IMarketingResultService _marketingResultService,
            IMarketingPlanService _marketingPlanService)
        {
            this._marketingResultService = _marketingResultService;
            this._marketingPlanService = _marketingPlanService;
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
                    list = list.Where(c => c.MarketingPlanID == planID);
                }

                var resultList = list.Select(c => new MarketingResultViewModel(c));
                return resultList;
            });
            return Ok(await task);
        }

        [Route("PostMarketingResults")]
        public IHttpActionResult PostMarketingResults(PostMarketingResultViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            //bool requestDone = _marketingResultService.CreateResults(request.ToMarketingResultModels(), request.IsFinished, request.StaffID);
            try
            {
                var foundPlan = _marketingPlanService.Get(request.PlanID);
                var addedResult = _marketingResultService.Add(request.ToResultModel());
                _marketingResultService.SaveChanges();
                return Ok(addedResult.ID);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
