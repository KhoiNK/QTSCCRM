using APIProject.Service;
using APIProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace APIProject.Controllers
{
    //[Authorize]
    [RoutePrefix("api/marketingplan")]
    public class MarketingPlanController : ApiController
    {
        private readonly IMarketingPlanService _marketingPlanService;

        public MarketingPlanController(IMarketingPlanService _marketingPlanService)
        {
            this._marketingPlanService = _marketingPlanService;
        }

        //[Authorize(Roles = "Admin,Employee")]
        [Route("GetMarketingPlanList")]
        [ResponseType(typeof(MarketingPlanViewModel))]
        public async Task<IHttpActionResult> GetMarketingPlanList()
        {
            var task = Task.Factory.StartNew(() =>
            {
                var list = _marketingPlanService.GetMarketingPlans().Select(c => new MarketingPlanViewModel(c));
                return list;
            });
            return Ok(await task);
        }

        [Route("GetMarketingPlan")]
        [ResponseType(typeof(MarketingPlanDetailViewModel))]
        public IHttpActionResult GetMarketingPlan(int? id)
        {
            if (id.HasValue)
            {
                var plan = _marketingPlanService.GetMarketingPlan(id.Value);
                if (plan != null)
                {
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
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }


            //check attach file here
            string budgetB64 = null;
            string eventB64 = null;
            string taskB64 = null;
            string licenseB64 = null;
            if (request.BudgetFile != null)
            {
                budgetB64 = request.BudgetFile.Base64Content;
            }
            if (request.EventScheduleFile != null)
            {
                eventB64 = request.EventScheduleFile.Base64Content;
            }
            if (request.TaskAssignFile != null)
            {
                taskB64 = request.TaskAssignFile.Base64Content;
            }
            if (request.LicenseFile != null)
            {
                licenseB64 = request.LicenseFile.Base64Content;
            }

            //insert plan and get plan id
            int requestID = _marketingPlanService.CreateNewPlan(request.ToMarketingPlanModel(), request.IsFinished,
                budgetB64, taskB64, eventB64, licenseB64);

            return Ok(requestID);
        }


        [Route("PutDraftingMarketingPlan")]
        public IHttpActionResult PutDraftingMarketingPlan([FromBody]PutDraftingMarketingPlanViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string budgetB64 = null;
            string eventB64 = null;
            string taskB64 = null;
            string licenseB64 = null;
            if (request.BudgetFile != null)
            {
                budgetB64 = request.BudgetFile.Base64Content;
            }
            if (request.EventScheduleFile != null)
            {
                eventB64 = request.EventScheduleFile.Base64Content;
            }
            if (request.TaskAssignFile != null)
            {
                taskB64 = request.TaskAssignFile.Base64Content;
            }
            if (request.LicenseFile != null)
            {
                licenseB64 = request.LicenseFile.Base64Content;
            }

            return Ok(_marketingPlanService.UpdatePlan(request.ToMarketingPlanModel(), request.IsFinished,
                budgetB64, taskB64, eventB64, licenseB64));

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
