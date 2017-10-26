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

            #region verify quote
            var foundQuote = _quoteService.Get(request.ID);
            if (foundQuote == null)
            {
                return BadRequest(message: CustomError.QuoteNotFound);
            }
            if (foundQuote.Status != QuoteStatus.Drafting)
            {
                return BadRequest(message: CustomError.QuoteStatusRequired + QuoteStatus.Drafting);
            }
            #endregion
            #region verify staff
            var foundStaff = _staffService.Get(request.StaffID);
            if (foundStaff == null)
            {
                return BadRequest(message: CustomError.StaffNotFound);
            }
            if (foundQuote.CreatedStaffID.Value != foundStaff.ID)
            {
                return BadRequest(message: CustomError.WrongAuthorizedStaff);
            }
            #endregion
            #region verify quote items
            var salesItemIDs = _salesItemService.GetAll().Where(c => c.IsDelete == false).Select(c => c.ID);
            if (salesItemIDs.Intersect(request.SalesItemIDs).Count() != request.SalesItemIDs.Count)
            {
                return BadRequest(CustomError.QuoteItemsNotFound);
            }
            #endregion

            foundQuote.Tax = request.Tax;
            foundQuote.Discount = foundQuote.Discount;
            _quoteService.Update(foundQuote);
            _quoteService.SaveChanges();

            var oldSalesItemIDs = foundQuote.QuoteItemMappings.Where(c=>c.IsDelete==false).Select(c => c.SalesItemID);
            var intersectPart = oldSalesItemIDs.Intersect(request.SalesItemIDs);
            var insertPart = request.SalesItemIDs.Except(intersectPart).ToList()
                .Select(c=>_salesItemService.Get(c));
            var deleteIDsPart = oldSalesItemIDs.Except(intersectPart).ToList();
            var deleteParts = foundQuote.QuoteItemMappings.Where(c => deleteIDsPart.Contains(c.SalesItemID)
            && c.IsDelete == false);
            foreach (var part in insertPart)
            {
                _quoteItemMappingService.Add(new QuoteItemMapping
                {
                    QuoteID=foundQuote.ID,
                    SalesItemID= part.ID,
                    SalesItemName= part.Name,
                    Price= part.Price,
                    Unit= part.Unit
                });
            }
            foreach(var quoteItem in deleteParts)
            {
                //_quoteItemMappingService.DeleteBySalesItemID(itemID);
                _quoteItemMappingService.Delete(quoteItem);
            }
            _quoteItemMappingService.SaveChanges();

            return Ok();
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
