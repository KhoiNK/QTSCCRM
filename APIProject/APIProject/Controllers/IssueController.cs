using APIProject.Service;
using APIProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;

namespace APIProject.Controllers
{
    [RoutePrefix("api/issue")]
    public class IssueController : ApiController
    {
        private readonly IIssueService _issueService;
        private readonly ISalesCategoryService _salesCategoryService;

        public IssueController(IIssueService _issueService, ISalesCategoryService _salesCategoryService)
        {
            this._issueService = _issueService;
            this._salesCategoryService = _salesCategoryService;
        }

        [Route("PostOpenIssue")]
        public IHttpActionResult PostOpenIssue(PostOpenIssueViewModel request)
        {
            if(!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            else
            {
                if (request.SalesCategoryIDs != null)
                {
                    List<int> categoryIDList = _salesCategoryService.GetAllCategories().Select(c => c.ID).ToList();
                    bool checkCateIDValid = categoryIDList.Intersect(request.SalesCategoryIDs).Count() == request.SalesCategoryIDs.Count();
                    if (!checkCateIDValid)
                    {
                        return BadRequest("Category IDs invalid");
                    }
                }
            }

            int insertedIssue = _issueService.CreateOpenIssue(request.ToIssueModel(), request.SalesCategoryIDs);
            return Ok(new { IssueID = insertedIssue });
            //return Json(new {IssueID = insertedIssue });
            //return insertedIssue;
        }

        [Route("GetIssues")]
        [ResponseType(typeof(IssueViewModel))]
        public IHttpActionResult GetIssues()
        {
            return Ok(_issueService.GetAllIssues().Select(c=> new IssueViewModel(c)));
        }

        [Route("GetIssueDetail")]
        [ResponseType(typeof(IssueDetailViewModel))]
        public IHttpActionResult GetIssueDetail(int id = 0)
        {
            if(id == 0)
            {
                return BadRequest();
            }

            var foundIssue = _issueService.GetAllIssues().Where(c => c.ID == id).SingleOrDefault();
            if(foundIssue != null)
            {
                return Ok(new IssueDetailViewModel(foundIssue));
            }
            return NotFound();
        }

        [Route("GetCustomerIssues")]
        [ResponseType(typeof(IssueDetailViewModel))]
        public IHttpActionResult GetCustomerIssues(int customerID = 0)
        {
            if (customerID == 0)
            {
                return BadRequest();
            }

            var foundIssues = _issueService.GetByCustomer(customerID);
            if(foundIssues != null)
            {
                return Ok(foundIssues.Select(c => new IssueDetailViewModel(c)));
            }
            return NotFound();
        }

        [Route("GetIssueStages")]
        public IHttpActionResult GetIssueStages()
        {
            return Ok(_issueService.GetStages());
        }

        [Route("GetIssueStatus")]
        public IHttpActionResult GetIssueStatus()
        {
            return Ok(_issueService.GetStatus());
        }
    }
}
