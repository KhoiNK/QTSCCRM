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

namespace APIProject.Controllers
{
    //[Authorize(Roles = "Kinh doanh")]
    [RoutePrefix("api/opportunity")]
    public class OpportunityController : ApiController
    {
        private readonly IOpportunityService _opportunityService;
        private readonly IUploadNamingService _uploadNamingService;
        private readonly ISalesCategoryService _salesCategoryService;
        private readonly IOpportunityCategoryMappingService _opportunityCategoryMappingService;

        public OpportunityController(IOpportunityService _opportunityService,
            IUploadNamingService _uploadNamingService,
            ISalesCategoryService _salesCategoryService,
            IOpportunityCategoryMappingService _opportunityCategoryMappingService)
        {
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

        //[Route("PutOpportunityInformation")]
        //public IHttpActionResult PutOpportunityInformation(PutOpportunityInformationViewModel request)
        //{
        //    if (!ModelState.IsValid || request == null)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    if (request.CategoryIDs.Any())
        //    {
        //        var categoryList = _salesCategoryService.GetAllCategories().Select(c => c.ID).ToList();
        //        if (categoryList.Intersect(request.CategoryIDs).Count() != request.CategoryIDs.Count())
        //        {
        //            return BadRequest("Invalid categories");
        //        }
        //    }
        //    try
        //    {
        //        _opportunityService.EditInfo(request.ToOpportunityModel());
        //        if (request.CategoryIDs.Any())
        //        {
        //            _opportunityCategoryMappingService.MapOpportunityCategories(request.ID, request.CategoryIDs);
        //        }

        //    }
        //    catch (Exception exceptionFromService)
        //    {
        //        return BadRequest(exceptionFromService.Message);
        //    }
           
        //}

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
    }
}
