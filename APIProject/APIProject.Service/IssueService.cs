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
    public interface IIssueService
    {
        int CreateOpenIssue(Issue issue, List<int> salesCategoryIDs);
    }
    public class IssueService : IIssueService
    {
        private readonly IIssueRepository _issueRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IContactRepository _contactRepository;
        private readonly ISalesCategoryRepository _salesCategoryRepository;
        private readonly IIssueCategoryMappingRepository _issueCategoryMappingRepository;
        private readonly IUnitOfWork _unitOfWork;

        private readonly string OpenStageName = "Chưa xử lý";
        public IssueService(IIssueRepository _issueRepository, IUnitOfWork _unitOfWork,
            ICustomerRepository _customerRepository, IContactRepository _contactRepository,
            IStaffRepository _staffRepository, ISalesCategoryRepository _salesCategoryRepository,
            IIssueCategoryMappingRepository _issueCategoryMappingRepository)
        {
            this._issueRepository = _issueRepository;
            this._customerRepository = _customerRepository;
            this._contactRepository = _contactRepository;
            this._staffRepository = _staffRepository;
            this._salesCategoryRepository = _salesCategoryRepository;
            this._issueCategoryMappingRepository = _issueCategoryMappingRepository;
            this._unitOfWork = _unitOfWork;
        }

        public int CreateOpenIssue(Issue issue, List<int> salesCategoryIDs)
        {
            Customer foundCustomer = _customerRepository.GetById(issue.CustomerID.Value);
            if (foundCustomer == null)
            {
                return 0;
            }
            else
            {
                if (foundCustomer.IsLead)
                {
                    return 0;
                }
            }
            if(_contactRepository.GetById(issue.ContactID.Value) == null)
            {
                return 0;
            }
            if(_staffRepository.GetById(issue.ModifiedStaffID.Value) == null)
            {
                return 0;
            }
            var distinctedCategories = salesCategoryIDs.Distinct();
            List<IssueCategoryMapping> categories = new List<IssueCategoryMapping>();
            foreach(int eachId in distinctedCategories)
            {
                SalesCategory foundCategory = _salesCategoryRepository.GetById(eachId);
                if(foundCategory == null)
                {
                    return 0;
                }
                categories.Add(new IssueCategoryMapping { SalesCategoryID = eachId, IsDeleted = false  });
            }

            issue.CreateStaffID = issue.ModifiedStaffID;
            issue.OpenStaffID = issue.ModifiedStaffID;
            issue.OpenedDate = DateTime.Today.Date;
            issue.ModifiedDate = DateTime.Today.Date;
            issue.Stage = OpenStageName;
            if(categories.Count > 0)
            {
                //issue.SalesCategories = categories;
                issue.IssueCategoryMappings = categories;
            }

            _issueRepository.Add(issue);
            _unitOfWork.Commit();
            return issue.ID;
        }
    }
}
