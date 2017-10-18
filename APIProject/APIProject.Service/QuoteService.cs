using APIProject.Data.Infrastructure;
using APIProject.Data.Repositories;
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
    }
    public class QuoteService : IQuoteService
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public QuoteService(IQuoteRepository _quoteRepository, IUnitOfWork _unitOfWork)
        {
            this._quoteRepository = _quoteRepository;
            this._unitOfWork = _unitOfWork;
        }
        public Quote GetOpportunityQuote(int opportunityID)
        {
            return _quoteRepository.GetLatestQuoteByOpportunity(opportunityID);
        }
    }
}
