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
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            else
            {
                if (request.CategoryIDs != null)
                {

                    if (request.Type != ActivityType.FromCustomer)
                    {
                        return BadRequest("Type 'Đến khách hàng' thì chưa có cơ hội bán hàng");
                    }

                    List<int> categoryIDList = _salesCategoryService.GetAllCategories().Select(c => c.ID).ToList();
                    bool checkCateIDValid = categoryIDList.Intersect(request.CategoryIDs).Count() == request.CategoryIDs.Count();
                    if (!checkCateIDValid)
                    {
                        return BadRequest("Category IDs lỗi");
                    }
                }
            }
            Activity newActivity = request.ToActivityModel();

            int InsertedActivityID = _activityService.CreateNewActivity(newActivity);
            if (InsertedActivityID == 0)
            {
                return BadRequest("Không thể tạo lịch hẹn, kiểm tra lại json khớp với business logic");
            }
            int? InsertedOpportunityID = null;
            //generate opp condition
            if (request.CategoryIDs != null)
            {
                var _insertedActivity = _activityService.GetAllActivities().Where(c => c.ID == InsertedActivityID).SingleOrDefault();
                Opportunity newOpportunity = new Opportunity
                {
                    ContactID = _insertedActivity.ContactID,
                    CreatedStaffID = _insertedActivity.CreateStaffID,
                    Title = _insertedActivity.Title,
                    Description = _insertedActivity.Description
                };

                InsertedOpportunityID = _opportunityService.CreateOpportunity(newOpportunity);
                _opportunityCategoryMappingService.MapOpportunityCategories(InsertedOpportunityID.Value,
                    request.CategoryIDs);
                bool mapOpportunityActivity = _opportunityService.MapOpportunityActivity(InsertedOpportunityID.Value, InsertedActivityID);
                //newActivity.OpportunityID = insertedOpportunity.ID;
                //return Ok(insertedOpportunityID);
            }
            //return Ok(insertedActivityID);
            return Ok(new { InsertedOpportunityID, InsertedActivityID });
        }

        [Route("PutSaveChangeActivity")]
        [ResponseType(typeof(Boolean))]
        public IHttpActionResult PutSaveChangeActivity(PutSaveChangeActivityViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            if (!ActivityMethod.GetList().Contains(request.Method))
            {
                return BadRequest();
            }

            bool Updated = _activityService.SaveChangeActivity(request.ToActivityModel());
            if (Updated)
            {
                return Ok(new { Updated });
            }
            return BadRequest("Không thể cập nhật, kiểm tra lại json phù hợp business logic");
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
                    return BadRequest("Category IDs sai");
                }

                bool IsOppActivity = _activityService.CheckIsOppActivity(request.ID);
                if (IsOppActivity)
                {
                    return BadRequest("Hoạt động của 1 cơ hội bán hàng không thể sinh ra cơ hội bán hàng khác");
                }
            }

            bool Updated = _activityService.CompleteActivity(request.ToActivityModel());
            if (!Updated)
            {
                return BadRequest("Không thể cập nhật, kiểm tra json phù hợp business logic");
            }
            int? InsertedOpportunityID = null;
            if (request.CategoryIDs != null)
            {
                Activity _updatedActivity = _activityService.GetAllActivities().Where(c => c.ID == request.ID).SingleOrDefault();
                Opportunity newOpportunity = new Opportunity
                {
                    ContactID = _updatedActivity.ContactID,
                    CreatedStaffID = _updatedActivity.CreateStaffID,
                    Title = _updatedActivity.Title,
                    Description = _updatedActivity.Description
                };

                InsertedOpportunityID = _opportunityService.CreateOpportunity(newOpportunity);
                _opportunityCategoryMappingService.MapOpportunityCategories(InsertedOpportunityID.Value,
                   request.CategoryIDs);
                bool mapOpportunityActivity = _opportunityService.MapOpportunityActivity(InsertedOpportunityID.Value, _updatedActivity.ID);
                //newActivity.OpportunityID = insertedOpportunity.ID;
                //return Ok(InsertedOpportunityID);
            }

            return base.Ok(new { Updated, InsertedOpportunityID });
        }

        [Route("PutCancelActivity")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult PutCancelActivity(PutCancelActivityViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            bool Updated = _activityService.CancelActivity(request.ToActivityModel());
            if (!Updated)
            {
                return BadRequest("Không thể cập nhật, kiểm tra json phù hợp business logic");
            }
            return Ok(new { Updated });
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
            return Ok(_activityService.GetAllActivities().Select(c => new ActivityViewModel(c)));
        }

        [Route("GetActivityDetail")]
        [ResponseType(typeof(ActivityDetailViewModel))]
        public IHttpActionResult GetActivityDetail(int? id)
        {
            if (id.HasValue)
            {
                return Ok(_activityService.GetAllActivities().Where(c => c.ID == id.Value)
                    .Select(c => new ActivityDetailViewModel(c)));
            }
            return BadRequest();
        }

        [Route("GetActivityDetails")]
        [ResponseType(typeof(ActivityDetailsViewModel))]
        public IHttpActionResult GetActivityDetail(int id = 0)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var foundActivity = _activityService.GetAllActivities().Where(c => c.ID == id).SingleOrDefault();
            if (foundActivity != null)
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
            if (opportunityID == 0)
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
