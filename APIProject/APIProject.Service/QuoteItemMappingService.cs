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
    public class QuoteItemMappingService: IQuoteItemMappingService
    {
        private readonly IQuoteItemMappingRepository _quoteItemMappingRepository;
        private readonly IUnitOfWork _unitOfWork;
        public QuoteItemMappingService(IQuoteItemMappingRepository _quoteItemMappingRepository,
            IUnitOfWork _unitOfWork)
        {
            this._quoteItemMappingRepository = _quoteItemMappingRepository;
            this._unitOfWork = _unitOfWork;
        }

        public void Add(QuoteItemMapping quoteItem)
        {
            quoteItem.CreatedDate = DateTime.Now;
            _quoteItemMappingRepository.Add(quoteItem);
        }
        void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        void IQuoteItemMappingService.SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }


    public interface IQuoteItemMappingService
    {
        void Add(QuoteItemMapping quoteItem);
        void SaveChanges();
    }
}
