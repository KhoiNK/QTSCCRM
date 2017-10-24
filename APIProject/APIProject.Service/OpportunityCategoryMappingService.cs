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
    public interface IOpportunityCategoryMappingService
    {
        void MapOpportunityCategories(int opportunityID, List<int> categoryIDs);
    }
    public class OpportunityCategoryMappingService : IOpportunityCategoryMappingService
    {
        private readonly IOpportunityCategoryMappingRepository _opportunityCategoryMappingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OpportunityCategoryMappingService(IOpportunityCategoryMappingRepository _opportunityCategoryMappingRepository,
            IUnitOfWork _unitOfWork)
        {
            this._opportunityCategoryMappingRepository = _opportunityCategoryMappingRepository;
            this._unitOfWork = _unitOfWork;
        }

        public void MapOpportunityCategories(int opportunityID, List<int> categoryIDs)
        {
            var foundCategoryList = _opportunityCategoryMappingRepository.GetByOpportunity(opportunityID);
            var intersectParts = foundCategoryList.Select(c => c.SalesCategoryID).ToList().Intersect(categoryIDs);
            var insertParts = categoryIDs.Except(intersectParts);
            var deleteParts = foundCategoryList.Select(c=>c.SalesCategoryID).ToList().Except(intersectParts);
            foreach(var foundCategory in foundCategoryList)
            {
                if (deleteParts.Contains(foundCategory.SalesCategoryID))
                {
                    foundCategory.IsDeleted = true;
                }
            }
            insertParts.ToList().ForEach(c =>
                    _opportunityCategoryMappingRepository.Add(new OpportunityCategoryMapping
                    {
                        SalesCategoryID = c,
                        OpportunityID = opportunityID,
                        IsDeleted = false,
                    }));
            _unitOfWork.Commit();
        }
    }
}
