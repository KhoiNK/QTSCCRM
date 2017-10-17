using APIProject.Data.Repositories;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Service
{
    public interface ISalesCategoryService
    {
        IEnumerable<SalesCategory> GetAllCategories();
    }
    public class SalesCategoryService: ISalesCategoryService
    {
        private readonly ISalesCategoryRepository _salesCategoryRepository;

        public SalesCategoryService(ISalesCategoryRepository _salesCategoryRepository)
        {
            this._salesCategoryRepository = _salesCategoryRepository;
        }

        public IEnumerable<SalesCategory> GetAllCategories()
        {
            return _salesCategoryRepository.GetAll();
        }
    }
}
