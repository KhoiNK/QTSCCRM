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
        IEnumerable<SalesItem> GetByCategory(int categoryID);
        int CreateNewItem(SalesItem salesItem);
    }
    public class SalesItemService: ISalesItemService
    {
        private readonly ISalesItemRepository _salesItemRepository;
        private readonly ISalesCategoryRepository _salesCategoryRepository ;
        private readonly IUnitOfWork _unitOfWork;

        public SalesItemService(ISalesItemRepository _salesItemRepository, IUnitOfWork _unitOfWork,
            ISalesCategoryRepository _salesCategoryRepository)
        {
            this._salesCategoryRepository = _salesCategoryRepository;
            this._salesItemRepository = _salesItemRepository;
            this._unitOfWork = _unitOfWork;
        }

        public int CreateNewItem(SalesItem salesItem)
        {
            var foundCategory = _salesCategoryRepository.GetById(salesItem.SalesCategoryID.Value);
            if(foundCategory != null)
            {
                _salesItemRepository.Add(salesItem);
                _unitOfWork.Commit();
                return salesItem.ID;
            }

            return 0;
        }

        public IEnumerable<SalesItem> GetByCategory(int categoryID)
        {
            var foundCategory = _salesCategoryRepository.GetById(categoryID);
            if (foundCategory != null)
            {
                var foundItems = foundCategory.SalesItems;
                if (foundItems.Any())
                {
                    return foundItems;
                }
            }
            return null;
        }

    }
}
