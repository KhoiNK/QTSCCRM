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
        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        
        public void DeleteBySalesItemID(int salesItemID)
        {
            var entity = _quoteItemMappingRepository.GetBySalesItemID(salesItemID);
            entity.UpdatedDate = DateTime.Now;
            entity.IsDelete = true;
            _quoteItemMappingRepository.Delete(entity);
        }

        public void Delete(QuoteItemMapping quoteItem)
        {
            var entity = _quoteItemMappingRepository.GetById(quoteItem.ID);
            entity = quoteItem;
            entity.UpdatedDate = DateTime.Now;
            entity.IsDelete = true;
            _quoteItemMappingRepository.Update(entity);
        }


    }


    public interface IQuoteItemMappingService
    {
        void Add(QuoteItemMapping quoteItem);
        void SaveChanges();
        void DeleteBySalesItemID(int salesItemID);
        void Delete(QuoteItemMapping quoteItem);
    }
}
