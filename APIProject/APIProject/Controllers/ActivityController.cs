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
    [RoutePrefix("api/activity")]
    public class ActivityController : ApiController
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService _activityService)
        {
            this._activityService = _activityService;
        }

        [Route("GetActivityTypes")]
        public IHttpActionResult GetActivityTypes()
        {
            return Ok(_activityService.GetActivityTypeNames());
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
            }
            int insertedActivity = _activityService.CreateNewActivity(request.ToActivityModel());
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
        public IHttpActionResult GetActivityDetail(int? id)
        {
            if (id.HasValue)
            {
                return Ok(_activityService.GetAllActivities().Where(c=>c.ID == id.Value)
                    .Select(c => new ActivityDetailViewModel(c)));
            }
            return BadRequest();
        }
    }
}
