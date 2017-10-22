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
        private readonly ISalesCategoryService _salesCategoryService;
        private readonly IUploadNamingService _uploadNamingService;

        public ActivityController(IActivityService _activityService,
            IOpportunityService _opportunityService,
            IOpportunityCategoryMappingService _opportunityCategoryMappingService,
            IUploadNamingService _uploadNamingService,
            ISalesCategoryService _salesCategoryService)
        {
            this._activityService = _activityService;
            this._opportunityService = _opportunityService;
            this._opportunityCategoryMappingService = _opportunityCategoryMappingService;
            this._uploadNamingService = _uploadNamingService;
            this._salesCategoryService = _salesCategoryService;
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
            
            int insertedActivityID = _activityService.CreateNewActivity(newActivity);
            if(insertedActivityID != 0)
            {
                //generate opp condition
                if (request.CategoryIDs != null)
                {
                    var _insertedActivity = _activityService.GetAllActivities().Where(c => c.ID == insertedActivityID).SingleOrDefault();
                    Opportunity newOpportunity = new Opportunity
                    {
                        ContactID = _insertedActivity.ContactID,
                        CreateStaffID = _insertedActivity.CreateStaffID,
                        Title = _insertedActivity.Title,
                        Description = _insertedActivity.Description
                    };

                    int insertedOpportunityID = _opportunityService.CreateOpportunity(newOpportunity);
                    bool insertedMapping = _opportunityCategoryMappingService.MapOpportunityCategories(insertedOpportunityID,
                        request.CategoryIDs);
                    bool mapOpportunityActivity = _opportunityService.MapOpportunityActivity(insertedOpportunityID, insertedActivityID);
                    //newActivity.OpportunityID = insertedOpportunity.ID;
                    return Ok(insertedOpportunityID);
                }
            }
            return Ok(insertedActivityID);
        }

        [Route("PutSaveChangeActivity")]
        [ResponseType(typeof(Boolean))]
        public IHttpActionResult PutSaveChangeActivity(PutSaveChangeActivityViewModel request)
        {
            if(!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            if (!ActivityMethod.GetList().Contains(request.Method))
            {
                return BadRequest();
            }

            bool saveOk = _activityService.SaveChangeActivity(request.ToActivityModel());
            return Ok(saveOk);
        }

        [Route("PutCompleteActivity")]
        [ResponseType(typeof(int))]
        public IHttpActionResult PutCompleteActivity(PutCompleteActivityViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            if (request.CategoryIDs != null)
            {
                List<int> categoryIDList = _salesCategoryService.GetAllCategories().Select(c => c.ID).ToList();
                bool checkCateIDValid = categoryIDList.Intersect(request.CategoryIDs).Count() == request.CategoryIDs.Count();
                if (!checkCateIDValid)
                {
                    return BadRequest("Category IDs invalid");
                }
            }

            bool updated = _activityService.CompleteActivity(request.ToActivityModel());
            if (updated)
            {
                if (request.CategoryIDs != null)
                {
                    Activity _updatedActivity = _activityService.GetAllActivities().Where(c => c.ID == request.ID).SingleOrDefault();
                    Opportunity newOpportunity = new Opportunity
                    {
                        ContactID = _updatedActivity.ContactID,
                        CreateStaffID = _updatedActivity.CreateStaffID,
                        Title = _updatedActivity.Title,
                        Description = _updatedActivity.Description
                    };

                    int insertedOpportunityID = _opportunityService.CreateOpportunity(newOpportunity);
                    bool insertedMapping = _opportunityCategoryMappingService.MapOpportunityCategories(insertedOpportunityID,
                        request.CategoryIDs);
                    bool mapOpportunityActivity = _opportunityService.MapOpportunityActivity(insertedOpportunityID, _updatedActivity.ID);
                    //newActivity.OpportunityID = insertedOpportunity.ID;
                    return Ok(insertedOpportunityID);
                }
                return Ok(CustomStatusCode.PutSuccess);
            }

            return base.Ok(CustomStatusCode.PutFailed);
        }

        [Route("PutCancelActivity")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult PutCancelActivity(PutCancelActivityViewModel request)
        {
            if(!ModelState.IsValid || request== null)
            {
                return BadRequest(ModelState);
            }

            bool cancelOk = _activityService.CancelActivity(request.ToActivityModel());
            return Ok(cancelOk);
        }

        //[Route("PutFinishActivity")]
        //public IHttpActionResult PutFinishActivity(PutFinishActivityViewModel request)
        //{
        //    if(!ModelState.IsValid || request == null)
        //    {
        //        return BadRequest(ModelState);
        //    }


        //    return Ok(_activityService.FinishActivity(request.ToActivityModel()));
        //}

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

        [Route("GetActivityDetails")]
        [ResponseType(typeof(ActivityDetailsViewModel))]
        public IHttpActionResult GetActivityDetail(int id = 0)
        {
            if(id == 0)
            {
                return BadRequest();
            }

            var foundActivity = _activityService.GetAllActivities().Where(c => c.ID == id).SingleOrDefault();
            if(foundActivity != null)
            {
                _uploadNamingService.ConcatContactAvatar(foundActivity.Contact);
                _uploadNamingService.ConcatCustomerAvatar(foundActivity.Customer);
                return Ok(new ActivityDetailsViewModel(foundActivity));
            }
            return NotFound();
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
