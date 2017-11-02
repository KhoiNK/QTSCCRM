using APIProject.GlobalVariables;
using APIProject.Helper;
using APIProject.Model.Models;
using APIProject.Service;
using APIProject.ViewModels;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
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
        private readonly IContactService _contactService;
        private readonly IQuoteItemMappingService _quoteItemMappingService;
        private readonly ISalesCategoryService _salesCategoryService;
        private readonly IUploadNamingService _uploadNamingService;

        public QuoteController(IQuoteService _quoteService, IOpportunityService _opportunityService,
            IStaffService _staffService, ISalesItemService _salesItemService,
            IQuoteItemMappingService _quoteItemMappingService,
            IContactService _contactService,
            ISalesCategoryService _salesCategoryService,
            IUploadNamingService _uploadNamingService
            )
        {
            this._uploadNamingService = _uploadNamingService;
            this._quoteService = _quoteService;
            this._contactService = _contactService;
            this._salesCategoryService = _salesCategoryService;
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

        [Route("GetQuote")]
        [ResponseType(typeof(QuoteDetailsViewModel))]
        public IHttpActionResult GetQuote(int id = 0)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            try
            {
                var foundOpp = _opportunityService.Get(id);
                var foundQuote = _quoteService.GetByOpportunity(foundOpp.ID);
                var quoteItems = _quoteItemMappingService.GetByQuote(foundQuote.ID);
                var oppCategories = _salesCategoryService.GetByOpportunity(foundQuote.OpportunityID);
                var response = new QuoteDetailsViewModel();
                response.ID = foundQuote.ID;
                response.Tax = foundQuote.Tax;
                response.Discount = foundQuote.Discount;
                response.Status = foundQuote.Status;
                response.UpdatedDate = foundQuote.UpdatedDate;
                response.SentCustomerDate = foundQuote.SentCustomerDate;
                if (foundQuote.ValidatedStaffID.HasValue)
                {
                    var validateStaff = _staffService.Get(foundQuote.ValidatedStaffID.Value);
                    _uploadNamingService.ConcatStaffAvatar(validateStaff);
                    response.ValidatedStaff = new StaffDetailViewModel(validateStaff);
                }
                response.Categories = new List<QuoteCategotyViewModel>();
                foreach (var category in oppCategories)
                {
                    var categoryItemIDs = _salesItemService.GetByCategory(category.ID).Select(c => c.ID);
                    var intersectQuoteItems = quoteItems.Where(c => categoryItemIDs.Contains(c.SalesItemID))
                        .Select(c => new QuoteItemViewModel(c));

                    response.Categories.Add(new QuoteCategotyViewModel
                    {
                        ID = category.ID,
                        Name = category.Name,
                        Items = intersectQuoteItems.ToList()
                    });
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("PostNewQuote")]
        [ResponseType(typeof(PostNewQuoteResponseViewModel))]
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

            try
            {
                var response = new PostNewQuoteResponseViewModel();
                var foundStaff = _staffService.Get(request.StaffID);
                var foundOpp = _opportunityService.Get(request.OpportunityID);
                var createdQuote = _quoteService.Add(request.ToQuoteModel(), request.SalesItemIDs);
                _quoteService.SaveChanges();
                response.QuoteCreated = true;
                response.QuoteID = createdQuote.ID;
                return Ok(response);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

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
        [ResponseType(typeof(PutUpdateQuoteResponseViewModel))]
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

        [Route("PutSendQuote")]
        public IHttpActionResult PutSendQuote(PutSendQuoteViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = new PutSendQuoteResponseViewModel();
                var foundQuote = _quoteService.Get(request.ID);
                var foundStaff = _staffService.Get(request.StaffID);
                var quoteOpp = _opportunityService.GetByQuote(request.ID);
                var oppContact = _contactService.Get(quoteOpp.ContactID.Value);
                //insert email service
                var quoteItems = _quoteItemMappingService.GetByQuote(request.ID);
                var pdfData = new List<string>();
                //initial header
                //string dataHeader = "Hạng mục;Giá thành;Đơn vị tính";
                //pdfData.Add(dataHeader);
                //foreach (var item in quoteItems)
                //{
                //    string itemLine = item.SalesItemName + ";"
                //        + string.Format("{0:n0}", item.Price) + ";" + item.Unit;
                //    pdfData.Add(itemLine);
                //}

                EmailHelper emailHelper = new EmailHelper();
                emailHelper.SendQuote(oppContact.Email, oppContact.Name, null);

                _quoteService.SetSend(request.ToQuoteModel());
                response.QuoteSent = true;

                _opportunityService.SetNextStage(quoteOpp);
                response.OpportunityUpdated = true;

                _opportunityService.SaveChanges();
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("PutSendQuote2")]
        public IHttpActionResult PutSendQuote2(PutSendQuoteViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            var response = new PutSendQuoteResponseViewModel();
            var foundQuote = _quoteService.Get(request.ID);
            var foundStaff = _staffService.Get(request.StaffID);
            var quoteOpp = _opportunityService.GetByQuote(request.ID);
            var oppContact = _contactService.Get(quoteOpp.ContactID.Value);
            //insert email service
            var quoteItems = _quoteItemMappingService.GetByQuote(request.ID);
            var pdfData = new List<string>();
            //initial header
            string dataHeader = "Hạng mục;Giá thành;Đơn vị tính";
            pdfData.Add(dataHeader);
            foreach (var item in quoteItems)
            {
                string itemLine = item.SalesItemName + ";"
                    + string.Format("{0:n0}", item.Price) + ";" + item.Unit;
                pdfData.Add(itemLine);
            }
            PdfHelper pdfHelper = new PdfHelper();
            PdfDocument doc = pdfHelper.CreateQuotePdf("Báo giá", pdfData, foundQuote.Tax, foundQuote.Discount);
            var quoteFileName =_uploadNamingService.GetQuoteNaming();
            SaveFileHelper saveFileHelper = new SaveFileHelper();
            var filepath = saveFileHelper.SavePdfQuoteFile(doc, quoteFileName);
            doc.Close();
            EmailHelper emailHelper = new EmailHelper();
            emailHelper.SendQuote(oppContact.Email, oppContact.Name, filepath);

            _quoteService.SetSend(request.ToQuoteModel());
            response.QuoteSent = true;

            _opportunityService.SetNextStage(quoteOpp);
            response.OpportunityUpdated = true;

            _opportunityService.SaveChanges();
            return Ok(response);

        }
        //[Route("GetTestFile")]
        //public IHttpActionResult GetTestFile()
        //{
        //    PdfHelper pdfHelper = new PdfHelper();
        //    //PdfDocument doc = pdfHelper.CreateQuotePdf("OYeah",1,1);
        //    string fileRoot = HttpContext.Current.Server.MapPath("~/Resources/CustomerAvatarFiles/");

        //    doc.SaveToFile(fileRoot + "abc.pdf");
        //    doc.Close();
        //    return Ok();
        //}
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
