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
        void Add(OpportunityCategoryMapping opportunityCategory);
        void UpdateRange(int opportunityID, List<int> categoryIDs);
        void Delete(OpportunityCategoryMapping oppCategory);

        void SaveChanges();
    }
    public class OpportunityCategoryMappingService : IOpportunityCategoryMappingService
    {
        private readonly IOpportunityService _opportunityService;
        private readonly IOpportunityCategoryMappingRepository _opportunityCategoryMappingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OpportunityCategoryMappingService(IOpportunityCategoryMappingRepository _opportunityCategoryMappingRepository,
            IOpportunityService _opportunityService, IUnitOfWork _unitOfWork)
        {
            this._opportunityService = _opportunityService;
            this._opportunityCategoryMappingRepository = _opportunityCategoryMappingRepository;
            this._unitOfWork = _unitOfWork;
        }

        public void Add(OpportunityCategoryMapping opportunityCategory)
        {
            opportunityCategory.CreatedDate = DateTime.Now;
            _opportunityCategoryMappingRepository.Add(opportunityCategory);
        }

        public void Delete(OpportunityCategoryMapping oppCategory)
        {
            var entity = _opportunityCategoryMappingRepository.GetById(oppCategory.ID);
            entity = oppCategory;
            entity.IsDelete = true;
            entity.UpdatedDate = DateTime.Now;
            _opportunityCategoryMappingRepository.Update(entity);
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
                    foundCategory.IsDelete = true;
                }
            }
            insertParts.ToList().ForEach(c =>
                    _opportunityCategoryMappingRepository.Add(new OpportunityCategoryMapping
                    {
                        SalesCategoryID = c,
                        OpportunityID = opportunityID,
                        IsDelete = false,
                    }));
            _unitOfWork.Commit();
        }
        public void UpdateRange(int opportunityID, List<int> categoryIDs)
        {
            var oldCategoryIDs = _opportunityCategoryMappingRepository.GetByOpportunity(opportunityID)
                .Where(c=>c.IsDelete==false).Select(c=>c.SalesCategoryID);
            var intersectIDs = oldCategoryIDs.Intersect(categoryIDs);
            var insertIDs = categoryIDs.Except(intersectIDs);
            var deleteIDs = oldCategoryIDs.Except(intersectIDs);

            foreach(var insertID in insertIDs)
            {
                Add(new OpportunityCategoryMapping
                {
                    OpportunityID=opportunityID,
                    SalesCategoryID=insertID
                });
            }
            var deleteEntities = _opportunityService.GetByID(opportunityID)
                .OpportunityCategoryMappings.Where(c=>c.IsDelete==false &&
                deleteIDs.Contains(c.SalesCategoryID));
            foreach(var deleteEntity in deleteEntities)
            {
                Delete(deleteEntity);
            }
        }
        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        
    }
}
