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
    public class QuoteItemMappingService: IQuoteItemMappingService
    {
        private readonly IQuoteItemMappingRepository _quoteItemMappingRepository;
        private readonly ISalesItemRepository _salesItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public QuoteItemMappingService(IQuoteItemMappingRepository _quoteItemMappingRepository,
            IUnitOfWork _unitOfWork, ISalesItemRepository _salesItemRepository)
        {
            this._quoteItemMappingRepository = _quoteItemMappingRepository;
            this._salesItemRepository = _salesItemRepository;
            this._unitOfWork = _unitOfWork;
        }

        public QuoteItemMapping Get(int quoteItemID)
        {
            var entity = _quoteItemMappingRepository.GetById(quoteItemID);
            return entity;
        }

        public IEnumerable<QuoteItemMapping> GetByQuote(int id)
        {
            var entities = _quoteItemMappingRepository.GetAll().Where(c => c.IsDelete == false
            && c.QuoteID == id);
            return entities;
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

        public void UpdateRange(int quoteID, List<int> itemIDs)
        {
            VerifySalesItemsExist(itemIDs);
            var oldItemEntities = _quoteItemMappingRepository.GetAll()
                .Where(c => c.QuoteID == quoteID && c.IsDelete == false);
            var intersectItemIDs = oldItemEntities.Select(c => c.SalesItemID)
                .Intersect(itemIDs);
            var deleteEntities = oldItemEntities.Where(c => !intersectItemIDs.Contains(c.SalesItemID));
            var insertItemIDs = itemIDs.Except(intersectItemIDs);
            foreach(var deleteEntity in deleteEntities)
            {
                Delete(deleteEntity);
            }
            foreach(var insertID in insertItemIDs)
            {
                var salesItemEntity = _salesItemRepository.GetById(insertID);
                Add(new QuoteItemMapping
                {
                    SalesItemID=insertID,
                    QuoteID=quoteID,
                    SalesItemName=salesItemEntity.Name,
                    Price=salesItemEntity.Price,
                    Unit=salesItemEntity.Unit,
                });
            }
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

        #region private verify
        private void VerifySalesItemsExist(List<int> itemIDs)
        {
            var salesItemEntities = _salesItemRepository.GetAll()
                .Where(c=>c.IsDelete==false).Select(c => c.ID);
            if (salesItemEntities.Intersect(itemIDs).Count() != itemIDs.Count)
            {
                throw new Exception(CustomError.QuoteItemsNotFound);
            }
        }
#endregion
    }


    public interface IQuoteItemMappingService
    {
        QuoteItemMapping Get(int quoteItemID);
        IEnumerable<QuoteItemMapping> GetByQuote(int id);
        void Add(QuoteItemMapping quoteItem);
        void UpdateRange(int quoteID, List<int> itemIDs);
        void DeleteBySalesItemID(int salesItemID);
        void Delete(QuoteItemMapping quoteItem);
        void SaveChanges();
    }
}
