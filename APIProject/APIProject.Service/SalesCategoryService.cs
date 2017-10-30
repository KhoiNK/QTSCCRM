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
        SalesCategory Get(int id);
        IEnumerable<SalesCategory> GetByIssue(int issueID);
        //IEnumerable<SalesCategory> GetByQuote(int quoteID);
        IEnumerable<SalesCategory> GetByOpportunity(int opportunityID);
        IEnumerable<SalesCategory> GetRange(List<int> IDs);
        void VerifyCategories(List<int> categoryIDs);
    }
    public class SalesCategoryService : ISalesCategoryService
    {
        private readonly ISalesCategoryRepository _salesCategoryRepository;
        private readonly IOpportunityCategoryMappingRepository _opportunityCategoryMappingRepository;
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly ISalesItemRepository _salesItemRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IQuoteItemMappingRepository _quoteItemMappingRepository;
        private readonly IIssueCategoryMappingRepository _issueCategoryMappingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SalesCategoryService(ISalesCategoryRepository _salesCategoryRepository,
            IOpportunityCategoryMappingRepository _opportunityCategoryMappingRepository,
            IOpportunityRepository _opportunityRepository, IUnitOfWork _unitOfWork,
            IIssueCategoryMappingRepository _issueCategoryMappingRepository,
            IQuoteRepository _quoteRepository,
            IQuoteItemMappingRepository _quoteItemMappingRepository,
            ISalesItemRepository _salesItemRepository)
        {
            this._opportunityCategoryMappingRepository = _opportunityCategoryMappingRepository;
            this._salesItemRepository = _salesItemRepository;
            this._quoteItemMappingRepository = _quoteItemMappingRepository;
            this._salesCategoryRepository = _salesCategoryRepository;
            this._opportunityRepository = _opportunityRepository;
            this._quoteRepository = _quoteRepository;
            this._issueCategoryMappingRepository = _issueCategoryMappingRepository;
            this._unitOfWork = _unitOfWork;
        }

        public SalesCategory Get(int id)
        {
            return _salesCategoryRepository.GetById(id);
        }
        public IEnumerable<SalesCategory> GetByIssue(int issueID)
        {
            var issueCategoryIDs = _issueCategoryMappingRepository.GetAll().Where(c => c.IssueID == issueID
            && c.IsDelete == false).Select(c=>c.SalesCategoryID);
            var categories = _salesCategoryRepository.GetAll().Where(c => issueCategoryIDs.Contains(c.ID));
            return categories;
        }
        //public IEnumerable<SalesCategory> GetByQuote(int quoteID)
        //{
        //    var quoteEntity = _quoteRepository.GetById(quoteID);
        //    var quoteSalesItemIDs = _quoteItemMappingRepository.GetByQuoteID(quoteID)
        //        .Select(c=>c.SalesItemID);
        //    foreach(var salesItemID in quoteSalesItemIDs)
        //    {

        //    }
        //    var salesCategoryIDs = _salesItemRepository.GetAll()
        //        .Where(c => quoteSalesItemIDs.Contains(c.ID)).Select(c=>c.SalesCategoryID).Distinct();
        //}

        public IEnumerable<SalesCategory> GetRange(List<int> IDs)
        {
            var entities = _salesCategoryRepository.GetAll().Where(
                c => IDs.Contains(c.ID) && c.IsDelete == false);
            if (entities.Any())
            {
                if (entities.Count() == IDs.Count)
                {
                    return entities;
                }
            }
            throw new Exception(CustomError.InvalidSalesCategories);
        }


        public IEnumerable<SalesCategory> GetAll()
        {
            return _salesCategoryRepository.GetAll();
        }

        public IEnumerable<SalesCategory> GetAllCategories()
        {
            return _salesCategoryRepository.GetAll();
        }

        public IEnumerable<SalesCategory> GetByOpportunity(int opportunityID)
        {
            var oppCategoryIDs = _opportunityCategoryMappingRepository.GetAll()
                .Where(c => c.OpportunityID == opportunityID&&c.IsDelete==false).Select(c => c.SalesCategoryID).ToList();
            return GetRange(oppCategoryIDs);
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
