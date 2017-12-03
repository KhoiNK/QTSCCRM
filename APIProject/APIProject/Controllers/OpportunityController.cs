using APIProject.GlobalVariables;
using APIProject.Model.Models;
using APIProject.Service;
using APIProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Description;

namespace APIProject.Controllers
{
    //[Authorize(Roles = "Kinh doanh")]
    [RoutePrefix("api/opportunity")]
    public class OpportunityController : ApiController
    {
        private readonly IOpportunityService _opportunityService;
        private readonly ICustomerService _customerService;
        private readonly IUploadNamingService _uploadNamingService;
        private readonly ISalesItemService _salesItemService;
        private readonly IStaffService _staffService;
        private readonly IContactService _contactService;
        private readonly IActivityService _activityService;
        private readonly IQuoteService _quoteService;
        private readonly ISalesCategoryService _salesCategoryService;
        private readonly IOpportunityCategoryMappingService _opportunityCategoryMappingService;

        public OpportunityController(IOpportunityService _opportunityService,
            IUploadNamingService _uploadNamingService,
            ISalesItemService _salesItemService,
            ICustomerService _customerService,
            IContactService _contactService,
            IActivityService _activityService,
            IStaffService _staffService,
            ISalesCategoryService _salesCategoryService,
            IQuoteService _quoteService,
            IOpportunityCategoryMappingService _opportunityCategoryMappingService)
        {
            this._salesItemService = _salesItemService;
            this._contactService = _contactService;
            this._quoteService = _quoteService;
            this._activityService = _activityService;
            this._staffService = _staffService;
            this._customerService = _customerService;
            this._opportunityService = _opportunityService;
            this._uploadNamingService = _uploadNamingService;
            this._salesCategoryService = _salesCategoryService;
            this._opportunityCategoryMappingService = _opportunityCategoryMappingService;
        }

        [Route("GetOpportunities")]
        [ResponseType(typeof(OpportunityViewModel))]
        public IHttpActionResult GetOpportunities(int page = 1,int pageSize =100)
        {
            var opportunities = _opportunityService.GetAll().Skip(pageSize * (page - 1)).Take(pageSize)
                .ToList();
            //opportunities.GroupBy(o => o.Customer).Select(c => c.Key)
            //    .ToList().ForEach(c => _uploadNamingService.ConcatCustomerAvatar(c));
            var responseList = new List<OpportunityViewModel>();
            foreach (var opp in opportunities)
            {
                var oppCus = _customerService.Get(opp.CustomerID.Value);
                var oppStaff = _staffService.Get(opp.CreatedStaffID.Value);
                _uploadNamingService.ConcatCustomerAvatar(oppCus);
                //var oppActivities = _activityService.GetByOpprtunity(opp.ID);
                var oppLastActivity = _activityService.GetByOpprtunity(opp.ID)
                    .Where(c => c.Status == ActivityStatus.Open ||
                    c.Status == ActivityStatus.Overdue).FirstOrDefault();
                responseList.Add(new OpportunityViewModel(opp,
                    oppCus,
                    oppLastActivity,
                    oppStaff));
            }
            return Ok(responseList);
        }

        [Route("GetOpportunityDetails")]
        [ResponseType(typeof(OpportunityDetailsViewModel))]
        public IHttpActionResult GetOpportunityDetails(int id = 0)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            try
            {
                var foundOpp = _opportunityService.Get(id);
                //var oppCus = _customerService.Get(foundOpp.CustomerID.Value);
                var oppContact = _contactService.Get(foundOpp.ContactID.Value);
                var oppCus = _customerService.Get(oppContact.CustomerID);
                var oppStaff = _staffService.Get(foundOpp.CreatedStaffID.Value);
                var oppActivities = _activityService.GetByOpprtunity(foundOpp.ID);
                var oppQuote = _quoteService.GetByOpportunity(foundOpp.ID);
                
                var oppCategories = _salesCategoryService.GetByOpportunity(foundOpp.ID);
                _uploadNamingService.ConcatContactAvatar(oppContact);
                _uploadNamingService.ConcatCustomerAvatar(oppCus);
                _uploadNamingService.ConcatStaffAvatar(oppStaff);
                var response = new OpportunityDetailsViewModel(foundOpp,
                    oppActivities.ToList(),
                    oppQuote,
                    oppStaff,
                    oppCus,
                    oppContact,
                    oppCategories.ToList());
                //if (response.QuoteDetail != null)
                //{
                //    foreach (var quoteItem in response.QuoteDetail.Items)
                //    {
                //        quoteItem.Name = quoteItem.Name;
                //    }
                //}
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("PutOpportunityInformation")]
        [ResponseType(typeof(PutOpportunityInformationResponseViewModel))]
        public IHttpActionResult PutOpportunityInformation(PutOpportunityInformationViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            #region validate category ids
            if (request.CategoryIDs != null)
            {
                if (!request.CategoryIDs.Any())
                {
                    return BadRequest(message: CustomError.OpportunitySalesCategoriesRequired);
                }
                if (request.CategoryIDs.Distinct().Count() != request.CategoryIDs.Count)
                {
                    return BadRequest(message: CustomError.OppCategoryNotDuplicateRequired);
                }
            }
            #endregion

            try
            {
                var response = new PutOpportunityInformationResponseViewModel();
                var foundOpp = _opportunityService.Get(request.ID);
                var foundStaff = _staffService.Get(request.StaffID);

                _opportunityService.UpdateInfo(request.ToOpportunityModel());
                response.BasicInfoUpdated = true;

                if (request.CategoryIDs != null)
                {
                    _opportunityCategoryMappingService.UpdateRange(foundOpp.ID, request.CategoryIDs);
                    response.CategoriesUpdated = true;
                }

                _opportunityService.SaveChanges();
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [Route("PutOpportunityNextStage")]
        [ResponseType(typeof(PutOpportunityNextStageResponseViewModel))]
        public IHttpActionResult PutOpportunityNextStage(PutOpportunityNextStageViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = new PutOpportunityNextStageResponseViewModel();
                var foundStaff = _staffService.Get(request.StaffID);
                var foundOpp = _opportunityService.Get(request.ID);
                foundOpp = _opportunityService.SetNextStage(request.ToOpportunityModel());
                response.OpportunityUpdated = true;

                if (foundOpp.StageName == OpportunityStage.ValidateQuote)
                {
                    var oppQuote = _quoteService.GetByOpportunity(foundOpp.ID);
                    _quoteService.SetValidatingStatus(oppQuote);
                    response.QuoteUpdated = true;
                }

                _opportunityService.SaveChanges();
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("PutWonOpportunity")]
        [ResponseType(typeof(PutWonOppResponseViewModel))]
        public IHttpActionResult PutWonOpportunity(PutWonOpportunityViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = new PutWonOppResponseViewModel();
                var foundStaff = _staffService.Get(request.StaffID);
                var foundOpportunity = _opportunityService.Get(request.ID);
                _opportunityService.SetWon(request.ToOpportunityModel());
                response.OpportunityUpdated = true;


                var oppCustomer = _customerService.GetByOpportunity(request.ID);
                if (oppCustomer.CustomerType == CustomerType.Lead)
                {
                    _customerService.ConvertToCustomer(oppCustomer);
                    response.CustomerConverted = true;
                }
                _customerService.SaveChanges();

                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("PutLostOpportunity")]
        [ResponseType(typeof(PutLostOppResponseViewModel))]
        public IHttpActionResult PutLostOpportunity(PutLostOpportunityViewModdel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = new PutLostOppResponseViewModel();
                var foundOpp = _opportunityService.Get(request.ID);
                _opportunityService.SetLost(request.ToOpportunityModel());
                _opportunityService.SaveChanges();
                response.OpportunityUpdated = true;
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("GetOpportunityStages")]
        public IHttpActionResult GetOpportunityStages()
        {
            var list = _opportunityService.GetStageDescription();
            var returnResult = new List<OpportunityStageViewModel>();
            foreach (var item in list)
            {
                returnResult.Add(new OpportunityStageViewModel { Name = item.Key, Description = item.Value });
            }
            return Ok(returnResult);
        }

        [Route("GetCustomerOpportunities")]
        [ResponseType(typeof(OpportunityDetailViewModel))]
        public IHttpActionResult GetCustomerOpportunities(int customerID = 0)
        {
            if (customerID == 0)
            {
                return BadRequest();
            }
            var foundOpportunities = _opportunityService.GetByCustomer(customerID);
            if (foundOpportunities != null)
            {
                return Ok(foundOpportunities.Select(c => new OpportunityDetailViewModel(c)));
            }

            return NotFound();
        }
        [Route("GetOpportunity")]
        [ResponseType(typeof(OpportunityDetailViewModel))]
        public IHttpActionResult GetOpportunity(int id = 0)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            return Ok(_opportunityService.GetAll().Where(c => c.ID == id)
                .Select(c => new OpportunityDetailViewModel(c)));
        }


    }
}
