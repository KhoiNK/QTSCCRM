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

namespace APIProject.Controllers
{
    [RoutePrefix("api/quote")]
    public class QuoteController : ApiController
    {
        private readonly IQuoteService _quoteService;
        private readonly IOpportunityService _opportunityService;
        private readonly IStaffService _staffService;
        private readonly ISalesItemService _salesItemService;
        private readonly IQuoteItemMappingService _quoteItemMappingService;

        public QuoteController(IQuoteService _quoteService, IOpportunityService _opportunityService,
            IStaffService _staffService, ISalesItemService _salesItemService,
            IQuoteItemMappingService _quoteItemMappingService)
        {
            this._quoteService = _quoteService;
            this._opportunityService = _opportunityService;
            this._staffService = _staffService;
            this._salesItemService = _salesItemService;
            this._quoteItemMappingService = _quoteItemMappingService;
        }

        [Route("GetOpportunityQuote")]
        public IHttpActionResult GetOpportunityQuote(int opportunityID = 0)
        {
            if (opportunityID == 0)
            {
                return BadRequest();
            }
            var foundQuote = _quoteService.GetOpportunityQuote(opportunityID);
            if (foundQuote != null)
            {
                return Ok(new QuoteViewModel(foundQuote));
            }
            else
            {
                return NotFound();
            }
        }

        [Route("PostNewQuote")]
        public IHttpActionResult PostNewQuote(PostNewQuoteViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            //var distinctIDs = new HashSet<int>(request.SalesItemIDs);
            //bool allDifferent = distinctIDs.Count == request.SalesItemIDs.Count;
            //if (!allDifferent)
            //{
            //    return BadRequest(CustomError.DuplicateIDs);
            //}

            //try
            //{
            //    var insertedQuoteID = _quoteService.CreateQuote(request.ToQuoteModel(), request.SalesItemIDs);
            //    return Ok(new { QuoteID = insertedQuoteID });
            //}
            //catch (Exception serviceException)
            //{
            //    return BadRequest(serviceException.Message);
            //}
            #region validate request
            if (request.SalesItemIDs.Distinct().Count() != request.SalesItemIDs.Count)
            {
                List<string> errors = new List<string>();
                return BadRequest(message: CustomError.QuoteItemNotDuplicateRequired);
            }

            #endregion

            #region check request
            var foundStaff = _staffService.Get(request.StaffID);
            if (foundStaff == null)
            {
                return BadRequest(message: CustomError.StaffNotFound);
            }
            if (foundStaff.Role.Name != RoleName.Sales)
            {
                return BadRequest(message: CustomError.StaffRoleRequired + RoleName.Sales);
            }

            var foundOpp = _opportunityService.Get(request.OpportunityID);
            if (foundOpp == null)
            {
                return BadRequest(message: CustomError.OpportunityNotFound);
            }
            if (foundOpp.StageName != OpportunityStage.MakeQuote)
            {
                return BadRequest(message: CustomError.OppStageRequired + OpportunityStage.MakeQuote);
            }
            if (foundOpp.Quotes.Any())
            {
                var lastQuote = foundOpp.Quotes.OrderByDescending(c => c.CreatedDate)
                    .Where(c => c.IsDelete == false).FirstOrDefault();
                if (lastQuote != null)
                {
                    if (lastQuote.Status == QuoteStatus.Drafting
                        || lastQuote.Status == QuoteStatus.Validating)
                    {
                        return BadRequest(message: CustomError.QuoteExisted);
                    }
                }
            }

            var salesItemIDs = _salesItemService.GetAll().Where(c => c.IsDelete == false).Select(c => c.ID);
            if (salesItemIDs.Intersect(request.SalesItemIDs).Count() != request.SalesItemIDs.Count)
            {
                return BadRequest(CustomError.QuoteItemsNotFound);
            }
            #endregion

            #region create new quote
            Quote newQuote = new Quote
            {
                CreatedStaffID = request.StaffID,
                OpportunityID = request.OpportunityID,
                Tax = request.Tax,
                Discount = request.Discount,
                Status=QuoteStatus.Drafting
            };

            _quoteService.Add(newQuote);
            _quoteService.SaveChanges();
            #endregion

            #region create and map quote items 
            foreach (var salesItemID in request.SalesItemIDs)
            {
                var foundItem = _salesItemService.Get(salesItemID);
                _quoteItemMappingService.Add(new QuoteItemMapping
                {
                    QuoteID = newQuote.ID,
                    SalesItemID = foundItem.ID,
                    SalesItemName = foundItem.Name,
                    Price = foundItem.Price,
                    Unit=foundItem.Unit
                });
            }
            _quoteItemMappingService.SaveChanges();
            #endregion
            return Ok(new { QuoteID = newQuote.ID });
        }

        [Route("PutValidQuote")]
        public IHttpActionResult PutValidQuote(PutValidQuoteViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            var foundQuote = _quoteService.Get(request.ID);
            if (foundQuote == null)
            {
                return BadRequest(message: CustomError.QuoteNotFound);
            }
            if (foundQuote.Status != QuoteStatus.Validating)
            {
                return BadRequest(message: CustomError.QuoteStatusRequired + QuoteStatus.Validating);
            }

            var foundStaff = _staffService.Get(request.StaffID);
            if (foundStaff == null)
            {
                return BadRequest(message: CustomError.StaffNotFound);
            }
            if (foundStaff.Role.Name != RoleName.Director)
            {
                return BadRequest(message: CustomError.WrongAuthorizedStaff);
            }

            foundQuote.Notes = request.Notes;
            foundQuote.Status = QuoteStatus.Valid;
            foundQuote.ValidatedStaffID = foundStaff.ID;

            _quoteService.Update(foundQuote);
            _quoteService.SaveChanges();


            var foundOpp = _opportunityService.Get(foundQuote.OpportunityID.Value);
            foundOpp.StageName = OpportunityStage.SendQuote;
            foundOpp.UpdatedStaffID = foundStaff.ID;

            _opportunityService.Update(foundOpp);
            _opportunityService.SaveChanges();

            //auto send mail - todo
            foundQuote.Status = QuoteStatus.SentCustomer;

            _quoteService.Update(foundQuote);
            _quoteService.SaveChanges();


            foundOpp.StageName = OpportunityStage.Negotiation;
            foundOpp.UpdatedStaffID = foundStaff.ID;

            _opportunityService.Update(foundOpp);
            _opportunityService.SaveChanges();

            return Ok();

        }

        [Route("PutInvalidQuote")]
        public IHttpActionResult PutInvalidQuote(PutInValidQuoteViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            var foundQuote = _quoteService.Get(request.ID);
            if (foundQuote == null)
            {
                return BadRequest(message: CustomError.QuoteNotFound);
            }
            if (foundQuote.Status != QuoteStatus.Validating)
            {
                return BadRequest(message: CustomError.QuoteStatusRequired + QuoteStatus.Validating);
            }

            var foundStaff = _staffService.Get(request.StaffID);
            if (foundStaff == null)
            {
                return BadRequest(message: CustomError.StaffNotFound);
            }
            if (foundStaff.Role.Name != RoleName.Director)
            {
                return BadRequest(message: CustomError.WrongAuthorizedStaff);
            }


            foundQuote.Notes = request.Notes;
            foundQuote.Status = QuoteStatus.NotValid;
            foundQuote.ValidatedStaffID = foundStaff.ID;

            _quoteService.Update(foundQuote);
            _quoteService.SaveChanges();


            var foundOpp = _opportunityService.Get(foundQuote.OpportunityID.Value);
            foundOpp.StageName = OpportunityStage.MakeQuote;
            foundOpp.UpdatedStaffID = foundStaff.ID;

            _opportunityService.Update(foundOpp);
            _opportunityService.SaveChanges();

            return Ok();
        }

    }
}
