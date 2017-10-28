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
using System.Web.Http.Description;

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
            var foundQuote = _quoteService.GetByOpportunity(opportunityID);
            if (foundQuote != null)
            {
                return Ok(new QuoteViewModel(foundQuote));
            }
            else
            {
                return NotFound();
            }
        }

        [Route("GetQuoteDetails")]
        [ResponseType(typeof(QuoteViewModel))]
        public IHttpActionResult GetQuoteDetails(int id = 0)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var foundQuote = _quoteService.Get(id);
            if (foundQuote == null)
            {
                return BadRequest(message: CustomError.QuoteNotFound);
            }

            return Ok(new QuoteViewModel(foundQuote));

        }

        [Route("PostNewQuote")]
        public IHttpActionResult PostNewQuote(PostNewQuoteViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            #region validate request
            if (!request.SalesItemIDs.Any())
            {
                return BadRequest(message: CustomError.QuoteItemRequired);
            }
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
                Status = QuoteStatus.Drafting
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
                    Unit = foundItem.Unit
                });
            }
            _quoteItemMappingService.SaveChanges();
            #endregion
            return Ok(new { QuoteID = newQuote.ID });
        }

        [Route("PutValidQuote")]
        [ResponseType(typeof(PutValidQuoteResponseViewModel))]
        public IHttpActionResult PutValidQuote(PutValidQuoteViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = new PutValidQuoteResponseViewModel();
                var foundQuote = _quoteService.Get(request.ID);
                var foundStaff = _staffService.Get(request.StaffID);
                _quoteService.SetValid(request.ToQuoteModel());
                response.QuoteUpdated = true;

                var quoteOpp = _opportunityService.GetByQuote(request.ID);
                quoteOpp.UpdatedStaffID = quoteOpp.CreatedStaffID;
                _opportunityService.SetNextStage(quoteOpp);
                response.OpportunityUpdated = true;
                _quoteService.SaveChanges();

                return Ok(response);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("PutInvalidQuote")]
        [ResponseType(typeof(PutInvalidQuoteResponseViewModel))]
        public IHttpActionResult PutInvalidQuote(PutInValidQuoteViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = new PutInvalidQuoteResponseViewModel();
                var foundQuote = _quoteService.Get(request.ID);
                var foundStaff = _staffService.Get(request.StaffID);

                _quoteService.SetInvalid(request.ToQuoteModel());
                response.QuoteUpdated = true;

                var quoteOpp = _opportunityService.GetByQuote(request.ID);
                quoteOpp.UpdatedStaffID = quoteOpp.CreatedStaffID;
                _opportunityService.SetMakeQuoteStage(quoteOpp);
                response.OpportunityUpdated = true;

                _quoteService.SaveChanges();
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [Route("PutUpdateQuote")]
        public IHttpActionResult PutUpdateQuote(PutUpdateQuoteViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            #region validate request
            if (!request.SalesItemIDs.Any())
            {
                return BadRequest(message: CustomError.QuoteItemRequired);
            }
            if (request.SalesItemIDs.Distinct().Count() != request.SalesItemIDs.Count)
            {
                List<string> errors = new List<string>();
                return BadRequest(message: CustomError.QuoteItemNotDuplicateRequired);
            }
            #endregion
            try
            {
                var response = new PutUpdateQuoteResponseViewModel();
                var foundQuote = _quoteService.Get(request.ID);
                var foundStaff = _staffService.Get(request.StaffID);

                _quoteService.UpdateInfo(request.ToQuoteModel());
                response.BasicInfoUpdated = true;

                _quoteItemMappingService.UpdateRange(request.ID, request.SalesItemIDs);
                response.QuoteItemsUpdated = true;
                _quoteItemMappingService.SaveChanges();
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }

        [Route("DeleteQuote")]
        public IHttpActionResult DeleteQuote(int id = 0)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var foundQuote = _quoteService.Get(id);
            if (foundQuote == null)
            {
                return BadRequest(message: CustomError.QuoteNotFound);
            }

            _quoteService.Delete(foundQuote);
            _quoteService.SaveChanges();

            //for debug
            var foundOpp = foundQuote.Opportunity;
            foundOpp.StageName = OpportunityStage.MakeQuote;
            _opportunityService.Update(foundOpp);
            _opportunityService.SaveChanges();

            return Ok();
        }

    }
}
