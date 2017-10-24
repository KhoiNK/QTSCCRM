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
        Quote GetOpportunityQuote(int opportunityID);
        void CreateQuote(Quote quote, List<int> quoteItems);
    }
    public class QuoteService : IQuoteService
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly ISalesItemRepository _salesItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public QuoteService(IQuoteRepository _quoteRepository, IUnitOfWork _unitOfWork,
            IOpportunityRepository _opportunityRepository,
            ISalesItemRepository _salesItemRepository)
        {
            this._quoteRepository = _quoteRepository;
            this._opportunityRepository = _opportunityRepository;
            this._salesItemRepository = _salesItemRepository;
            this._unitOfWork = _unitOfWork;
        }

        public void CreateQuote(Quote quote, List<int> quoteItems)
        {
            string errorMessage = CheckCreateQuoteRequest(quote, quoteItems);
            if (errorMessage != null)
            {
                throw new Exception(errorMessage);
            }

            quote.CreatedDate = DateTime.Now;
            quote.QuoteItemMappings = CreateQuoteItems(quoteItems);
            _quoteRepository.Add(quote);
            _unitOfWork.Commit();
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
            if (foundOpportunity.CreateStaffID != quote.CreatedStaffID)
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
    }
}
