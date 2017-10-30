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
        private readonly IStaffService _staffService;
        private readonly IOpportunityCategoryMappingService _opportunityCategoryMappingService;
        private readonly ISalesCategoryService _salesCategoryService;
        private readonly IUploadNamingService _uploadNamingService;

        public ActivityController(IActivityService _activityService,
            IOpportunityService _opportunityService, IStaffService _staffService,
            IOpportunityCategoryMappingService _opportunityCategoryMappingService,
            IUploadNamingService _uploadNamingService,
            ISalesCategoryService _salesCategoryService)
        {
            this._activityService = _activityService;
            this._opportunityService = _opportunityService;
            this._staffService = _staffService;
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
        [Route("GetActivityList")]
        public IHttpActionResult GetActivityList()
        {
            return Ok(_activityService.GetAllActivities().Select(c => new ActivityViewModel(c)));
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
        [Route("PostNewActivity")]
        [ResponseType(typeof(PostNewActivityResponseViewModel))]
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
                    List<int> categoryIDList = _salesCategoryService.GetAllCategories().Select(c => c.ID).ToList();
                    bool checkCateIDValid = categoryIDList.Intersect(request.CategoryIDs).Count() == request.CategoryIDs.Count();
                    if (!checkCateIDValid)
                    {
                        return BadRequest(message: CustomError.OppCategoriesNotFound);
                    }
                }
            }
            #region validate activty
            List<string> RequiredTypes = new List<string>
            {
                ActivityType.FromCustomer,
                ActivityType.ToCustomer
            };
            if (!RequiredTypes.Contains(request.Type))
            {
                return BadRequest(message: CustomError.ActivityTypesRequired +
                    String.Join(", ", RequiredTypes));
            }
            List<string> RequiredMethods = new List<string>
            {
                ActivityMethod.Direct,
                ActivityMethod.Email,
                ActivityMethod.Phone
            };
            if (!RequiredMethods.Contains(request.Method))
            {
                return BadRequest(message: CustomError.ActivityMethodsRequired
                    + String.Join(", ", RequiredMethods));
            }
            if (request.Type == ActivityType.FromCustomer)
            {
                if (DateTime.Compare(DateTime.Now, request.TodoTime) < 0)
                {
                    return BadRequest(message: CustomError.ActivityTodoNotPassCurrent);
                }
            }
            else
            {
                if (DateTime.Compare(DateTime.Now, request.TodoTime) > 0)
                {
                    return BadRequest(message: CustomError.ActivityTodoMustPassCurrent);
                }
                if (request.CategoryIDs != null)
                {
                    return BadRequest(message: CustomError.TypeToCustomerNotHaveCategories);
                }
            }
            #endregion
            try
            {
                if (request.CategoryIDs != null)
                {
                    _salesCategoryService.VerifyCategories(request.CategoryIDs);
                    //get range
                }
                var response = new PostNewActivityResponseViewModel();
                var insertedActivity = _activityService.Add(request.ToActivityModel());
                response.ActivityCreated = true;
                response.ActivityID = insertedActivity.ID;
                if (request.CategoryIDs != null)
                {
                    var actOpp = new Opportunity
                    {
                        Title = request.Title,
                        Description = request.Description,
                        CreatedStaffID = request.StaffID,
                        ContactID = request.ContactID,
                    };
                    var insertedOpp = _opportunityService.Add(actOpp);
                    _opportunityCategoryMappingService.AddRange(insertedOpp.ID, request.CategoryIDs);
                    _activityService.MapOpportunity(insertedActivity, insertedOpp);

                    response.OpportunityCreated = true;
                    response.OpportunityID = insertedOpp.ID;
                }
                _activityService.SaveChanges();
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Route("PutSaveChangeActivity")]
        [ResponseType(typeof(PutSaveChangeActivityResponseViewModel))]
        public IHttpActionResult PutSaveChangeActivity(PutSaveChangeActivityViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            List<string> RequiredMethods = new List<string>
            {
                ActivityMethod.Direct,
                ActivityMethod.Email,
                ActivityMethod.Phone
            };
            if(DateTime.Compare(DateTime.Now, request.TodoTime) > 0)
            {
                return BadRequest(message: CustomError.ActivityTodoMustPassCurrent);
            }
            if (!RequiredMethods.Contains(request.Method))
            {
                return BadRequest(message: CustomError.ActivityMethodsRequired
                    + String.Join(", ", RequiredMethods));
            }
            try
            {
                var response = new PutSaveChangeActivityResponseViewModel();
                var foundActivity = _activityService.Get(request.ID);
                var foundStaff = _staffService.Get(request.StaffID);
                _activityService.UpdateInfo(request.ToActivityModel());
                response.ActivityUpdated = true;
                _activityService.SaveChanges();
                return Ok(response);
            }catch(Exception e)
            {
                return BadRequest(e.Message);

            }

        }
        [Route("PutCompleteActivity")]
        [ResponseType(typeof(PutCompleteActivityResponseViewModel))]
        public IHttpActionResult PutCompleteActivity(PutCompleteActivityViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var foundActivity = _activityService.Get(request.ID);
                var foundStaff = _staffService.Get(request.StaffID);
                
                var response = new PutCompleteActivityResponseViewModel();
                _activityService.SetComplete(request.ToActivityModel());
                response.ActivityUpdated = true;
                if (request.CategoryIDs != null)
                {
                    _salesCategoryService.VerifyCategories(request.CategoryIDs);
                    _activityService.VerifyCanCreateOpportunity(request.ToActivityModel());

                    var newOpp = request.ToOpportunityModel();
                    newOpp.ContactID = foundActivity.ContactID;

                    var insertedOpp = _opportunityService.Add(newOpp);
                    _opportunityCategoryMappingService.AddRange(insertedOpp.ID, request.CategoryIDs);
                    _activityService.MapOpportunity(request.ToActivityModel(), insertedOpp);
                    response.OpportunityCreated = true;
                    response.OpportunityID = insertedOpp.ID;
                }
                _activityService.SaveChanges();
                return Ok(response);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("PutCancelActivity")]
        [ResponseType(typeof(PutCancelActivityResponseViewModel))]
        public IHttpActionResult PutCancelActivity(PutCancelActivityViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = new PutCancelActivityResponseViewModel();
                var foundActivity = _activityService.Get(request.ID);
                var foundStaff = _staffService.Get(request.StaffID);
                _activityService.SetCancel(request.ToActivityModel());
                response.ActivityUpdated = true;
                _activityService.SaveChanges();
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
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
