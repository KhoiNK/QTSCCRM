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
        void AddRange(Issue issue, List<SalesCategory> categories);
    }
    public class IssueCategoryMappingService: IIssueCategoryMappingService
    {
        private readonly IIssueCategoryMappingRepository _issueCategoryMappingRepository;
        private readonly IUnitOfWork _unitOfWork;
        public IssueCategoryMappingService(IIssueCategoryMappingRepository _issueCategoryMappingRepository,
            IUnitOfWork _unitOfWork)
        {
            this._issueCategoryMappingRepository = _issueCategoryMappingRepository;
            this._unitOfWork = _unitOfWork;
        }
        public void AddRange(Issue issue, List<SalesCategory> categories)
        {
            foreach(var category in categories)
            {
                _issueCategoryMappingRepository.Add(new IssueCategoryMapping
                {
                    IssueID=issue.ID,
                    SalesCategoryID=category.ID
                });
            }
        }


    }
}
