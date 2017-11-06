using APIProject.GlobalVariables;
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
        private readonly IUploadNamingService _uploadNamingService;
        private readonly ISalesCategoryService _salesCategoryService;
        private readonly IStaffService _staffService;
        private readonly IContactService _contactService;
        private readonly ICustomerService _customerService;
        private readonly IIssueCategoryMappingService _issueCategoryMappingService;

        public IssueController(IIssueService _issueService, ISalesCategoryService _salesCategoryService,
            IUploadNamingService _uploadNamingService,
            IStaffService _staffService,
            ICustomerService _customerService,
            IContactService _contactService,
            IIssueCategoryMappingService _issueCategoryMappingService)
        {
            this._issueService = _issueService;
            this._customerService = _customerService;
            this._salesCategoryService = _salesCategoryService;
            this._uploadNamingService = _uploadNamingService;
            this._staffService = _staffService;
            this._contactService = _contactService;
            this._issueCategoryMappingService = _issueCategoryMappingService;
        }

        
        [Route("GetIssues")]
        [ResponseType(typeof(IssueViewModel))]
        public IHttpActionResult GetIssues(int page=1, int pageSize=10)
        {
            var list = _issueService.GetAll().Skip(pageSize * (page - 1)).Take(pageSize);
            return Ok(list.Select(c => new IssueViewModel(c)));
        }

        [Route("GetIssueDetails")]
        [ResponseType(typeof(IssueDetailsViewModel))]
        public IHttpActionResult GetIssueDetails(int id = 0)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            try
            {
                var foundIssue = _issueService.Get(id);
                var issueCategories = _salesCategoryService.GetByIssue(foundIssue.ID);
                var issueContact = _contactService.Get(foundIssue.ContactID.Value);
                var issueCustomer = _customerService.Get(foundIssue.CustomerID.Value);
                _uploadNamingService.ConcatContactAvatar(issueContact);
                _uploadNamingService.ConcatCustomerAvatar(issueCustomer);
                var response = new IssueDetailsViewModel(foundIssue,
                    issueContact, issueCustomer, issueCategories.ToList());
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("PostOpenIssue")]
        [HttpPost]
        [ResponseType(typeof(PostOpenIssueResponseViewModel))]
        public IHttpActionResult PostOpenIssue([FromBody] PostOpenIssueViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            #region validate
            if (request.SolveDate.HasValue)
            {
                if (DateTime.Compare(DateTime.Now, request.SolveDate.Value) > 0)
                {
                    return BadRequest(message: CustomError.IssueSolveDateMustPassCurrent);
                }
            }
            if (!request.SalesCategoryIDs.Any())
            {
                return BadRequest(message: CustomError.IssueCategoriesRequired);
            }
            #endregion
            try
            {
                var foundCategories = _salesCategoryService.GetRange(request.SalesCategoryIDs);

                var response = new PostOpenIssueResponseViewModel();
                var foundStaff = _staffService.Get(request.StaffID);
                var foundContact = _contactService.Get(request.ContactID);

                var insertedIssue = _issueService.Add(request.ToIssueModel());
                _issueCategoryMappingService.AddRange(insertedIssue, foundCategories.ToList());
                response.IssueCreated = true;
                response.IssueID = insertedIssue.ID;

                if (request.SolveDate.HasValue)
                {
                    insertedIssue.SolveDate = request.SolveDate;
                    _issueService.SetSolve(insertedIssue);
                    response.IssueUpdated = true;
                }
                _issueService.SaveChanges();
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("PutUpdateIssue")]
        [ResponseType(typeof(PutUpdateIssueResponseViewModel))]
        public IHttpActionResult PutUpdateIssue(PutUpdateIssueViewModel request)
        {
            if(!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            #region validate
            if (request.SolveDate.HasValue)
            {
                if (DateTime.Compare(DateTime.Now, request.SolveDate.Value) > 0)
                {
                    return BadRequest(message: CustomError.IssueSolveDateMustPassCurrent);
                }
            }
            #endregion
            try
            {
                var response = new PutUpdateIssueResponseViewModel();
                var foundIssue = _issueService.Get(request.ID);
                var foundStaff = _issueService.Get(request.StaffID);
                _issueService.UpdateInfo(request.ToIssueModel());

                if (request.SolveDate.HasValue)
                {
                    _issueService.SetSolve(request.ToIssueModel());
                }
                _issueService.SaveChanges();
                response.IssueUpdated = true;
                return Ok(response);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("PutDoneIssue")]
        public IHttpActionResult PutDoneIssue(PutDoneIssueViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = new PutDoneIssueResponseViewModel();
                var foundIssue = _issueService.Get(request.ID);
                var foundStaff = _issueService.Get(request.StaffID);
                _issueService.SetDone(request.ToIssueModel());
                response.IssueUpdated = true;
                _issueService.SaveChanges();
                return Ok(response);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("PutFailIssue")]
        [ResponseType(typeof(PutFailIssueResponseViewModel))]
        public IHttpActionResult PutFailIssue(PutFailIssueViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = new PutFailIssueResponseViewModel();
                var foundIssue = _issueService.Get(request.ID);
                var foundStaff = _staffService.Get(request.StaffID);
                _issueService.SetFail(request.ToIssueModel());
                response.IssueUpdated = true;
                _issueService.SaveChanges();
                return Ok(response);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
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
            if (foundIssues != null)
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

        [Route("GetIssueDetail")]
        [ResponseType(typeof(IssueDetailViewModel))]
        public IHttpActionResult GetIssueDetail(int id = 0)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var foundIssue = _issueService.GetAllIssues().Where(c => c.ID == id).SingleOrDefault();
            if (foundIssue != null)
            {
                return Ok(new IssueDetailViewModel(foundIssue));
            }
            return NotFound();
        }
    }
}
