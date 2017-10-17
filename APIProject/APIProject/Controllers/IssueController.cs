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
    [RoutePrefix("api/issue")]
    public class IssueController : ApiController
    {
        private readonly IIssueService _issueService;

        public IssueController(IIssueService _issueService)
        {
            this._issueService = _issueService;
        }

        [Route("PostOpenIssue")]
        public IHttpActionResult PostOpenIssue(PostOpenIssueViewModel request)
        {
            if(!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            int insertedIssue = _issueService.CreateOpenIssue(request.ToIssueModel(), request.SalesCategoryIDs);
            return Ok(insertedIssue);
        }

        [Route("GetIssues")]
        public IHttpActionResult GetIssues()
        {
            return Ok(_issueService.GetAllIssues().Select(c=> new IssueViewModel(c)));
        }
    }
}
