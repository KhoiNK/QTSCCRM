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
    public class MarketingPlanController : ApiController
    {
        private readonly IMarketingPlanService _marketingPlanService;

        public MarketingPlanController(IMarketingPlanService _marketingPlanService)
        {
            this._marketingPlanService = _marketingPlanService;
        }

        [Route("GetMarketingPlanList")]
        public async Task<IHttpActionResult> GetMarketingPlanList()
        {
            var task = Task.Factory.StartNew(() => {
                var list = _marketingPlanService.GetMarketingPlans().Select(c => new MarketingPlanViewModel()
                {
                    ID = c.ID,
                    Title = c.Title,
                    CreatedDate = c.CreatedDate,
                    Stage = c.Stage,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    LastModifiedDate = c.LastModifiedDate,
                    LastModifiedStaffName = c.ModifiedStaff.Name,
                    Description = c.Description,
                    Budget = c.Budget,
                    Status = c.Status
                });
                return list;
            });
            return Ok(await task);
        }

        [Route("GetMarketingPlan")]
        public IHttpActionResult GetMarketingPlan(int? id)
        {
            if (id.HasValue)
            {
                var plan = _marketingPlanService.GetMarketingPlans().Where(x => x.ID == id).SingleOrDefault();

                //return _marketingPlanService.GetMarketingPlans(id).Select(c => new MarketingPlanDetailViewModel()
                //{
                //    ID = c.ID,
                //    Title = c.Title,
                //    CreatedDate = c.CreatedDate,
                //    Stage = c.Stage,
                //    StartDate = c.StartDate,
                //    EndDate = c.EndDate,
                //    LastModifiedDate = c.LastModifiedDate,
                //    LastModifiedStaffName = c.ModifiedStaff.Name,
                //    Description = c.Description,
                //    Budget = c.Budget,
                //    ValidateStaffName = c.Staff2.Name
                //});
                if (plan != null) {
                    MarketingPlanDetailViewModel item = new MarketingPlanDetailViewModel(plan);
                    return Ok(item);
                }
                else
                {
                    return NotFound();
                }
            }
            return BadRequest();
        }

        [Route("PostMarketingPlan")]
        public IHttpActionResult PostMarketingPlan([FromBody]PostMarketingPlanViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _marketingPlanService.CreateNewPlan(request.ToMarketingPlanModel(), request.IsFinished);

            return Ok();
        }

        [Route("PutDraftingMarketingPlan")]
        public IHttpActionResult PutDraftingMarketingPlan([FromBody]PutDraftingMarketingPlanViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(_marketingPlanService.UpdatePlan(request.ToMarketingPlanModel(), request.IsFinished));

        }

        [Route("PutValidateMarketingPlan")]
        public IHttpActionResult PutValidateMarketingPlan([FromBody]PutValidateMarketingPlanViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(_marketingPlanService.ValidatePlan(request.ToMarketingPlanModel(), request.Validate));
        }

        [Route("PutAcceptMarketingPlan")]
        public IHttpActionResult PutAcceptMarketingPlan([FromBody]PutAcceptMarketingPlanViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_marketingPlanService.AcceptPlan(request.ToMarketingPlanModel(), request.Accept));
        }

        [Route("DeleteMarketingPlan")]
        public IHttpActionResult DeleteMarketingPlan()
        {
            return Ok();
        }

    }
}
