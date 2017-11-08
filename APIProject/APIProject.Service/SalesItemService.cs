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
    public interface ISalesItemService
    {
        int CreateNewItem(SalesItem salesItem);
        void UpdateInfo(SalesItem salesItem);
        IEnumerable<SalesItem> GetAll();
        SalesItem Get(int id);
        IEnumerable<SalesItem> GetByCategory(int categoryID);
        void SaveChanges();
    }
    public class SalesItemService: ISalesItemService
    {
        private readonly ISalesItemRepository _salesItemRepository;
        private readonly IQuoteItemMappingRepository _quoteItemMappingRepository;
        private readonly ISalesCategoryRepository _salesCategoryRepository ;
        private readonly IUnitOfWork _unitOfWork;

        public SalesItemService(ISalesItemRepository _salesItemRepository, IUnitOfWork _unitOfWork,
            ISalesCategoryRepository _salesCategoryRepository,
            IQuoteItemMappingRepository _quoteItemMappingRepository)
        {
            this._quoteItemMappingRepository = _quoteItemMappingRepository;
            this._salesCategoryRepository = _salesCategoryRepository;
            this._salesItemRepository = _salesItemRepository;
            this._unitOfWork = _unitOfWork;
        }

        public int CreateNewItem(SalesItem salesItem)
        {
            var foundCategory = _salesCategoryRepository.GetById(salesItem.SalesCategoryID.Value);
            if(foundCategory != null)
            {
                salesItem.CreatedDate = DateTime.Now;
                _salesItemRepository.Add(salesItem);
                _unitOfWork.Commit();
                return salesItem.ID;
            }

            return 0;
        }

        public void UpdateInfo(SalesItem salesItem)
        {
            var entity = _salesItemRepository.GetById(salesItem.ID);
            entity.Name = salesItem.Name;
            entity.Price = salesItem.Price;
            entity.Unit = salesItem.Unit;
            entity.UpdatedDate = DateTime.Now;
            _salesItemRepository.Update(entity);
        }
        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }


        public SalesItem Get(int id)
        {
            return _salesItemRepository.GetById(id);
        }

        public IEnumerable<SalesItem> GetAll()
        {
            return _salesItemRepository.GetAll();
        }

        public IEnumerable<SalesItem> GetByCategory(int categoryID)
        {
            var entities = _salesItemRepository.GetAll()
                .Where(c => c.IsDelete == false && c.SalesCategoryID == categoryID);
            return entities;
        }


    }
}
