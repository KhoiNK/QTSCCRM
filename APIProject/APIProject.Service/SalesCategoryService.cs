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
    public interface ISalesCategoryService
    {
        IEnumerable<SalesCategory> GetAll();
        IEnumerable<SalesCategory> GetAllCategories();
        IEnumerable<SalesCategory> GetByOpportunity(int opportunityID);
        IEnumerable<SalesCategory> GetByIssue(int issueID);
        SalesCategory Get(int id);

        void VerifyCategories(List<int> categoryIDs);
    }
    public class SalesCategoryService : ISalesCategoryService
    {
        private readonly ISalesCategoryRepository _salesCategoryRepository;
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly IIssueCategoryMappingRepository _issueCategoryMappingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SalesCategoryService(ISalesCategoryRepository _salesCategoryRepository,
            IOpportunityRepository _opportunityRepository, IUnitOfWork _unitOfWork,
            IIssueCategoryMappingRepository _issueCategoryMappingRepository)
        {
            this._salesCategoryRepository = _salesCategoryRepository;
            this._opportunityRepository = _opportunityRepository;
            this._issueCategoryMappingRepository = _issueCategoryMappingRepository;
            this._unitOfWork = _unitOfWork;
        }

        public SalesCategory Get(int id)
        {
            return _salesCategoryRepository.GetById(id);
        }

        public IEnumerable<SalesCategory> GetAll()
        {
            return _salesCategoryRepository.GetAll();
        }

        public IEnumerable<SalesCategory> GetAllCategories()
        {
            return _salesCategoryRepository.GetAll();
        }

        public IEnumerable<SalesCategory> GetByIssue(int issueID)
        {
            var mappings = _issueCategoryMappingRepository.GetAll().Where(c => c.IssueID == issueID
            && c.IsDeleted == false);
            if (mappings.Any())
            {
                return mappings.Select(c => c.SalesCategory);
            }
            return null;
        }

        public IEnumerable<SalesCategory> GetByOpportunity(int opportunityID)
        {
            var foundOpportunity = _opportunityRepository.GetById(opportunityID);
            if (foundOpportunity != null)
            {
                var mappings = foundOpportunity.OpportunityCategoryMappings;
                if (mappings.Any())
                {
                    return mappings.Where(c=>c.IsDelete == false).Select(c => c.SalesCategory);
                }
            }
            return null;
        }

        public void VerifyCategories(List<int> categoryIDs)
        {
            var categoryEntityIDs = _salesCategoryRepository.GetAll()
                .Where(c => c.IsDelete == false).Select(c => c.ID);
            if (categoryEntityIDs.Intersect(categoryIDs).Count()
                != categoryIDs.Count)
            {
                throw new Exception(CustomError.OppCategoriesNotFound);
            }
        }

    }
}
