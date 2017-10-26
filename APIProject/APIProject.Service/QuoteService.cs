using APIProject.Data.Infrastructure;
using APIProject.Data.Repositories;
using APIProject.GlobalVariables;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Service
{
    public interface IQuoteService
    {
        Quote Get(int id);

        Quote GetOpportunityQuote(int opportunityID);
        int CreateQuote(Quote quote, List<int> quoteItems);
        Quote ValidateQuote(int quoteID, bool isValid, int staffID, string notes);
        void SaveChanges();
        void Update(Quote quote);
        void Add(Quote quote);
        void Delete(Quote quote);
    }
    public class QuoteService : IQuoteService
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ISalesItemRepository _salesItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public QuoteService(IQuoteRepository _quoteRepository, IUnitOfWork _unitOfWork,
            IOpportunityRepository _opportunityRepository,
            ISalesItemRepository _salesItemRepository,
            IStaffRepository _staffRepository)
        {
            this._quoteRepository = _quoteRepository;
            this._opportunityRepository = _opportunityRepository;
            this._salesItemRepository = _salesItemRepository;
            this._staffRepository = _staffRepository;
            this._unitOfWork = _unitOfWork;
        }

        public int CreateQuote(Quote quote, List<int> quoteItems)
        {
            string errorMessage = CheckCreateQuoteRequest(quote, quoteItems);
            if (errorMessage != null)
            {
                throw new Exception(errorMessage);
            }

            quote.CreatedDate = DateTime.Now;
            quote.Status = QuoteStatus.Drafting;
            quote.QuoteItemMappings = CreateQuoteItems(quoteItems);
            _quoteRepository.Add(quote);
            _unitOfWork.Commit();
            return quote.ID;
        }

        private List<QuoteItemMapping> CreateQuoteItems(List<int> quoteItemIDs)
        {
            List<QuoteItemMapping> _list = new List<QuoteItemMapping>();
            foreach (var itemID in quoteItemIDs)
            {
                var foundItem = _salesItemRepository.GetById(itemID);
                _list.Add(new QuoteItemMapping
                {
                    SalesItemID = itemID,
                    Price = foundItem.Price,
                    Unit = foundItem.Unit,
                    SalesItemName = foundItem.Name
                });
            }
            return _list;
        }

        private string CheckCreateQuoteRequest(Quote quote, List<int> quoteItems)
        {
            var foundOpportunity = _opportunityRepository.GetById(quote.OpportunityID.Value);
            if (foundOpportunity == null)
            {
                return CustomError.OpportunityNotFound;
            }
            if (foundOpportunity.CreatedStaffID != quote.CreatedStaffID)
            {
                return CustomError.WrongAuthorizedStaff;
            }
            var lastQuote = _quoteRepository.GetLatestQuoteByOpportunity(foundOpportunity.ID);
            if (lastQuote != null)
            {
                if (lastQuote.Status != QuoteStatus.NotValid)
                {
                    return CustomError.PendingQuoteExisted;
                }
            }
            var SalesItemIDs = _salesItemRepository.GetAll().Select(c => c.ID).ToList();
            bool okToInsertItems = SalesItemIDs.Intersect(quoteItems).Count() == quoteItems.Count;
            if (!okToInsertItems)
            {
                return CustomError.InvalidSalesItems;
            }
            return null;
        }

        public Quote GetOpportunityQuote(int opportunityID)
        {
            return _quoteRepository.GetLatestQuoteByOpportunity(opportunityID);
        }

        private string CheckValidateQuoteRequest(Quote foundQuote, Staff foundStaff)
        {
            if(foundQuote == null)
            {
                return CustomError.QuoteNotFound;
            }else if(foundQuote.Status != QuoteStatus.Validating)
            {
                return CustomError.ValidatingQuoteRequired;
            }
            if(foundStaff == null)
            {
                return CustomError.StaffNotFound;
            }
            else if(foundStaff.Role.Name != RoleName.Director)
            {
                return CustomError.WrongAuthorizedStaff;
            }
            return null;
        }

        public Quote ValidateQuote(int quoteID, bool isValid, int staffID, string notes)
        {
            var foundQuote = _quoteRepository.GetById(quoteID);
            var foundStaff = _staffRepository.GetById(staffID);
            string errorMessage = CheckValidateQuoteRequest(foundQuote, foundStaff);
            if(errorMessage != null)
            {
                throw new Exception(errorMessage);
            }

            if (isValid)
            {
                foundQuote.Status = QuoteStatus.Valid;
                foundQuote.ValidatedStaffID = foundStaff.ID;
            }
            else
            {
                foundQuote.Status = QuoteStatus.NotValid;
            }
            foundQuote.Notes = notes;
            foundQuote.UpdatedDate = DateTime.Now;
            _unitOfWork.Commit();
            //later send quote function

            return foundQuote;
        }


        private void SendQuoteToCustomer(Quote quote)
        {
            quote.SentCustomerDate = DateTime.Now;
            _unitOfWork.Commit();
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public Quote Get(int id)
        {
            return _quoteRepository.GetById(id);
        }

        public void Update(Quote quote)
        {
            var entity = _quoteRepository.GetById(quote.ID);
            entity = quote;
            entity.UpdatedDate = DateTime.Now;
            _quoteRepository.Update(entity);
        }

        public void Add(Quote quote)
        {
            quote.CreatedDate = DateTime.Now;
            _quoteRepository.Add(quote);
        }

        public void Delete(Quote quote)
        {
            var entity = _quoteRepository.GetById(quote.ID);
            entity = quote;
            entity.IsDelete = true;
            _quoteRepository.Update(entity);
        }
    }
}
