using APIProject.GlobalVariables;
using APIProject.Model.Models;
using APIProject.Service;
using APIProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace APIProject.Controllers
{
    [RoutePrefix("api/activity")]
    public class ActivityController : ApiController
    {
        private readonly IActivityService _activityService;
        private readonly IOpportunityService _opportunityService;
        private readonly IOpportunityCategoryMappingService _opportunityCategoryMappingService;

        public ActivityController(IActivityService _activityService,
            IOpportunityService _opportunityService,
            IOpportunityCategoryMappingService _opportunityCategoryMappingService)
        {
            this._activityService = _activityService;
            this._opportunityService = _opportunityService;
            this._opportunityCategoryMappingService = _opportunityCategoryMappingService;
        }

        [Route("GetActivityTypes")]
        public IHttpActionResult GetActivityTypes()
        {
            return Ok(_activityService.GetActivityTypeNames());
        }

        [Route("GetActivityStatus")]
        public IHttpActionResult GetActivityStatus()
        {
            return Ok(_activityService.GetActivityStatusNames());
        }

        [Route("GetActivityMethods")]
        public IHttpActionResult GetActivityMethods()
        {
            return Ok(_activityService.GetActivityMethodNames());
        }

        [Route("PostNewActivity")]
        public IHttpActionResult PostNewActivity(PostNewActivityViewModel request)
        {
            if(!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }else
            {
                if(request.CategoryIDs != null)
                {
                    if(request.Type != ActivityType.FromCustomer)
                    {
                        return BadRequest("To customer type can't generate opportunity at create");
                    }
                }
            }
            Activity newActivity = request.ToActivityModel();
            if (request.CategoryIDs != null)
            {

                Opportunity newOpportunity = new Opportunity
                {
                    CustomerID=request.CustomerID,
                    ContactID=request.ContactID,
                    CreateStaffID=request.StaffID,
                    Title=request.Title,
                    Description=request.Description
                };
                
                Opportunity insertedOpportunity = _opportunityService.CreateOpportunity(newOpportunity);
                bool insertedMapping = _opportunityCategoryMappingService.MapOpportunityCategories(insertedOpportunity.ID, 
                    request.CategoryIDs);
                newActivity.OpportunityID = insertedOpportunity.ID;
            }
            int insertedActivity = _activityService.CreateNewActivity(newActivity);

            return Ok(insertedActivity);
        }

        [Route("PutFinishActivity")]
        public IHttpActionResult PutFinishActivity(PutFinishActivityViewModel request)
        {
            if(!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            
            return Ok(_activityService.FinishActivity(request.ToActivityModel()));
        }

        [Route("GetActivityList")]
        public IHttpActionResult GetActivityList()
        {
            return Ok(_activityService.GetAllActivities().Select(c=> new ActivityViewModel(c)));
        }

        [Route("GetActivityDetail")]
        [ResponseType(typeof(ActivityDetailViewModel))]
        public IHttpActionResult GetActivityDetail(int? id)
        {
            if (id.HasValue)
            {
                return Ok(_activityService.GetAllActivities().Where(c=>c.ID == id.Value)
                    .Select(c => new ActivityDetailViewModel(c)));
            }
            return BadRequest();
        }

        [Route("GetOpportunityActivities")]
        [ResponseType(typeof(ActivityDetailViewModel))]
        public IHttpActionResult GetOpportunityActivities(int opportunityID = 0)
        {
            if(opportunityID == 0)
            {
                return BadRequest();
            }
            var foundActivities = _activityService.GetByOpprtunity(opportunityID);
            if (foundActivities != null)
            {
                return Ok(foundActivities.Select(c => new ActivityDetailViewModel(c)));
            }
            return NotFound();
        }

        [Route("GetCustomerActivities")]
        [ResponseType(typeof(ActivityDetailViewModel))]
        public IHttpActionResult GetCustomerActivities(int customerID = 0)
        {
            if (customerID == 0)
            {
                return BadRequest();
            }
            var foundActivities = _activityService.GetByCustomer(customerID);
            if (foundActivities != null)
            {
                return Ok(foundActivities.Select(c => new ActivityDetailViewModel(c)));
            }
            return NotFound();
        }

    }
}
