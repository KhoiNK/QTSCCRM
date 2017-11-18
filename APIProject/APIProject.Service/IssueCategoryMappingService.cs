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
    public interface IIssueCategoryMappingService
    {
        Dictionary<string, int> GetCounts(int monthRange);
        void AddRange(Issue issue, List<SalesCategory> categories);
    }
    public class IssueCategoryMappingService: IIssueCategoryMappingService
    {
        private readonly IIssueCategoryMappingRepository _issueCategoryMappingRepository;
        private readonly ISalesCategoryRepository _salesCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public IssueCategoryMappingService(IIssueCategoryMappingRepository _issueCategoryMappingRepository,
            ISalesCategoryRepository _salesCategoryRepository,
            IUnitOfWork _unitOfWork)
        {
            this._issueCategoryMappingRepository = _issueCategoryMappingRepository;
            this._salesCategoryRepository = _salesCategoryRepository;
            this._unitOfWork = _unitOfWork;
        }

        public Dictionary<string, int> GetCounts(int monthRange)
        {
            var categoryEntities = _salesCategoryRepository.GetAll().Where(c=>c.IsDelete==false);
            Dictionary<string, int> response = new Dictionary<string, int>();
            foreach(var categoryEntity in categoryEntities)
            {
                var value = _issueCategoryMappingRepository.GetAll().Where(c => c.IsDelete == false
                && c.SalesCategoryID == categoryEntity.ID
                && (DateTime.Now.Subtract(c.CreatedDate.Value).Days/(365.2425/12)) <= monthRange).Count();
                response.Add(categoryEntity.Name, value);
            }
            return response;
        }

        public void AddRange(Issue issue, List<SalesCategory> categories)
        {
            foreach(var category in categories)
            {
                _issueCategoryMappingRepository.Add(new IssueCategoryMapping
                {
                    IssueID=issue.ID,
                    SalesCategoryID=category.ID,
                    CreatedDate=DateTime.Now
                });
            }
        }


    }
}
