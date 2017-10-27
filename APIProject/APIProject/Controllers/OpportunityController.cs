﻿using APIProject.GlobalVariables;
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

        [HttpPut]
        [Route("PutOpportunityInformation")]
        public IHttpActionResult PutOpportunityInformation(PutOpportunityInformationViewModel request)
        {
            //User.Identity.Name;


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
            #region verify category ids
            var dbCategoryIDs = _salesCategoryService.GetAll().Where(c => c.IsDelete == false)
                .Select(c => c.ID);
            if (dbCategoryIDs.Intersect(request.CategoryIDs).Count() != request.CategoryIDs.Count)
            {
                return BadRequest(message: CustomError.OppCategoriesNotFound);
            }
            #endregion

            try
            {
                var foundOpp = _opportunityService.Get(request.ID);
                var foundStaff = _staffService.Get(request.StaffID);
                var CanChangeInfoStages = new List<string>
                {
                    OpportunityStage.Consider,
                    OpportunityStage.MakeQuote,
                    OpportunityStage.ValidateQuote,
                    OpportunityStage.SendQuote,
                    OpportunityStage.Negotiation
                };
                if (!CanChangeInfoStages.Contains(foundOpp.StageName))
                {
                    return BadRequest(message: CustomError.ChangeInfoStageRequired +
                        String.Join(", ", CanChangeInfoStages));
                }
                var CanChangeCategoriesStages = new List<string>
                {
                    OpportunityStage.Consider,
                    OpportunityStage.MakeQuote
                };
                if (!CanChangeCategoriesStages.Contains(foundOpp.StageName))
                {
                    return BadRequest(message: CustomError.ChangeCategoryStageRequired +
                        String.Join(", ", CanChangeCategoriesStages));
                }
                foundOpp.UpdatedStaffID = request.StaffID;
                foundOpp.Title = request.Title;
                foundOpp.Description = request.Description;
                _opportunityService.Update(foundOpp);


                if (request.CategoryIDs != null)
                {
                    _opportunityCategoryMappingService.UpdateRange(foundOpp.ID, request.CategoryIDs);
                }
                _opportunityService.SaveChanges();
                //_opportunityCategoryMappingService.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [Route("PutOpportunityNextStage")]
        public IHttpActionResult PutOpportunityNextStage(PutOpportunityNextStageViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            #region verify staff
            var foundStaff = _staffService.Get(request.StaffID);
            if (foundStaff == null)
            {
                return BadRequest(message: CustomError.StaffNotFound);
            }
            #endregion
            #region verify opportunity
            var foundOpp = _opportunityService.Get(request.ID);
            if (foundOpp == null)
            {
                return BadRequest(message: CustomError.OpportunityNotFound);
            }
            List<string> RequiredStages = new List<string>
            {
                OpportunityStage.Consider,
                OpportunityStage.MakeQuote,
                OpportunityStage.ValidateQuote,
                OpportunityStage.SendQuote,
            };
            if (!RequiredStages.Contains(foundOpp.StageName))
            {
                return BadRequest(message: CustomError.OppStageRequired +
                    String.Join(", ", RequiredStages));
            }
            var foundQuote = foundOpp.Quotes.Where(c => c.IsDelete == false)
                    .OrderByDescending(c => c.CreatedDate).FirstOrDefault();
            if (foundOpp.StageName == OpportunityStage.MakeQuote)
            {
                if (foundQuote == null)
                {
                    return BadRequest(message: CustomError.CreateQuoteRequired);
                }
            }
            if (foundOpp.StageName == OpportunityStage.ValidateQuote)
            {
                if (foundQuote.Status != QuoteStatus.Valid)
                {
                    return BadRequest(message: CustomError.QuoteStatusRequired + QuoteStatus.Valid);
                }
            }
            if (foundOpp.StageName == OpportunityStage.SendQuote)
            {
                if (!foundQuote.SentCustomerDate.HasValue)
                {
                    return BadRequest(message: CustomError.SendQuoteRequired);
                }
            }
            #endregion

            foundOpp.StageName = NextStageName(foundOpp.StageName);
            foundOpp.UpdatedStaffID = request.StaffID;
            _opportunityService.Update(foundOpp);
            _opportunityService.SaveChanges();
            return Ok();


        }

        [Route("PutWonOpportunity")]
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

            var foundOpportunity = _opportunityService.Get(request.ID);
            if (foundOpportunity == null)
            {
                return BadRequest(message: CustomError.OpportunityNotFound);
            }
            if (foundOpportunity.StageName != OpportunityStage.Negotiation)
            {
                return BadRequest(message: CustomError.OppStageRequired + OpportunityStage.Negotiation);
            }


            foundOpportunity.UpdatedStaffID = request.StaffID;
            _opportunityService.SetWon(foundOpportunity);
            _opportunityService.Update(foundOpportunity);
            _opportunityService.SaveChanges();


            var foundCustomer = _customerService.Get(foundOpportunity.CustomerID.Value);
            _customerService.ConvertToCustomer(foundCustomer);
            _customerService.Update(foundCustomer);
            _customerService.SaveChanges();

            return Ok();
        }

        [Route("PutLostOpportunity")]
        public IHttpActionResult PutLostOpportunity(PutLostOpportunityViewModdel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            var foundOpp = _opportunityService.Get(request.ID);
            if (foundOpp == null)
            {
                return BadRequest(message: CustomError.OpportunityNotFound);
            }

            return Ok();
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

        private String NextStageName(string currentStageName)
        {
            string result;
            if (currentStageName == OpportunityStage.Consider)
            {
                result = OpportunityStage.MakeQuote;
            }
            else if (currentStageName == OpportunityStage.MakeQuote)
            {
                result = OpportunityStage.ValidateQuote;
            }
            else if (currentStageName == OpportunityStage.ValidateQuote)
            {
                result = OpportunityStage.SendQuote;
            }
            else
            {
                result = OpportunityStage.Negotiation;
            }
            return result;
        }
    }
}
