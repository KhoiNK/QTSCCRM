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
    [RoutePrefix("api/staff")]
    public class StaffController : ApiController
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService _staffService)
        {
            this._staffService = _staffService;
        }

        [Route("GetOpportunityStaff")]
        [ResponseType(typeof(StaffDetailViewModel))]
        public IHttpActionResult GetOpportunityStaff(int opportunityID = 0)
        {
            if(opportunityID == 0)
            {
                return BadRequest();
            }
            var foundStaff = _staffService.GetByOpportunity(opportunityID);
            if(foundStaff != null)
            {
                return Ok(new StaffDetailViewModel(foundStaff));
            }
            else
            {
                return NotFound();
            }
        }

        [Route("GetActivityStaff")]
        [ResponseType(typeof(StaffDetailViewModel))]
        public IHttpActionResult GetActivityStaff(int activityID = 0)
        {
            if (activityID == 0)
            {
                return BadRequest();
            }
            var foundStaff = _staffService.GetByActivity(activityID);
            if (foundStaff != null)
            {
                return Ok(new StaffDetailViewModel(foundStaff));
            }
            else
            {
                return NotFound();
            }
        }

        [Route("GetStaffs")]
        [ResponseType(typeof(StaffDetailViewModel))]
        public IHttpActionResult GetStaffs()
        {
            var staffs = _staffService.GetAllStaffs();
            return Ok(staffs.Select(c => new StaffDetailViewModel(c)));
        }
    }
}
