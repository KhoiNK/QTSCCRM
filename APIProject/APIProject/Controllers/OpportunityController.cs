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
        private readonly IStaffService _staffService;
        private readonly IQuoteService _quoteService;
        private readonly ISalesCategoryService _salesCategoryService;
        private readonly IOpportunityCategoryMappingService _opportunityCategoryMappingService;

        public OpportunityController(IOpportunityService _opportunityService,
            IUploadNamingService _uploadNamingService,
            ICustomerService _customerService,
            IStaffService _staffService,
            ISalesCategoryService _salesCategoryService,
            IQuoteService _quoteService,
            IOpportunityCategoryMappingService _opportunityCategoryMappingService)
        {
            this._quoteService = _quoteService;
            this._staffService = _staffService;
            this._customerService = _customerService;
            this._opportunityService = _opportunityService;
            this._uploadNamingService = _uploadNamingService;
            this._salesCategoryService = _salesCategoryService;
            this._opportunityCategoryMappingService = _opportunityCategoryMappingService;
        }

        [Route("GetOpportunities")]
        [ResponseType(typeof(OpportunityViewModel))]
        public IHttpActionResult GetOpportunities()
        {
            //return Ok(_opportunityService.GetAllOpportunities().Select(c => new OpportunityViewModel(c)));
            var opportunities = _opportunityService.GetAllOpportunities();
            var customers = opportunities.GroupBy(o => o.Customer).Select(c => c.Key);
            foreach (var customer in customers)
            {
                _uploadNamingService.ConcatCustomerAvatar(customer);
            }
            return Ok(opportunities.Select(c => new OpportunityViewModel(c)));
        }

        
        [Route("GetOpportunityDetails")]
        [ResponseType(typeof(OpportunityDetailsViewModel))]
        public IHttpActionResult GetOpportunityDetails(int id = 0)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var foundOpp = _opportunityService.GetByID(id);
            if (foundOpp != null)
            {
                _uploadNamingService.ConcatContactAvatar(foundOpp.Contact);
                _uploadNamingService.ConcatCustomerAvatar(foundOpp.Customer);
                return Ok(new OpportunityDetailsViewModel(foundOpp));
                //return Ok(_opportunityService.GetAllOpportunities().Where(c => c.ID == id)
                //.Select(c => new OpportunityDetailsViewModel(c)));
            }
            return NotFound();
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
                //_opportunityCategoryMappingService.SaveChanges();
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

                if(foundOpp.StageName == OpportunityStage.ValidateQuote)
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

            var foundStaff = _staffService.Get(request.StaffID);
            if (foundStaff == null)
            {
                return BadRequest(message: CustomError.StaffNotFound);
            }


            try
            {
                var response = new PutWonOppResponseViewModel();
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
            }catch(Exception e)
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
            return Ok(_opportunityService.GetAllOpportunities().Where(c => c.ID == id)
                .Select(c => new OpportunityDetailViewModel(c)));
        }


    }
}
