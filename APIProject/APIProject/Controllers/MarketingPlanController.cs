﻿using APIProject.Service;
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
    [RoutePrefix("api/marketingplan")]
    public class MarketingPlanController : ApiController
    {
        private readonly IMarketingPlanService _marketingPlanService;

        public MarketingPlanController(IMarketingPlanService _marketingPlanService)
        {
            this._marketingPlanService = _marketingPlanService;
        }

        [Route("GetMarketingPlanList")]
        [ResponseType(typeof(MarketingPlanViewModel))]
        public async Task<IHttpActionResult> GetMarketingPlanList()
        {
            var task = Task.Factory.StartNew(() => {
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

            int requestID = _marketingPlanService.CreateNewPlan(request.ToMarketingPlanModel(), request.IsFinished);

            return Ok(requestID);
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