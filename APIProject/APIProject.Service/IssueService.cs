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
    public interface IIssueService
    {
        int CreateOpenIssue(Issue issue, List<int> salesCategoryIDs);
        IEnumerable<Issue> GetAllIssues();
        IEnumerable<Issue> GetByCustomer(int customerID);
        List<string> GetStages();
        List<string> GetStatus();

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
            Customer foundCustomer = _customerRepository.GetByContact(issue.ContactID.Value);
            if (foundCustomer == null)
            {
                return 0;
            }
            else
            {
                if (foundCustomer.CustomerType == CustomerType.Lead)
                {
                    return 0;
                }
            }
            if (_staffRepository.GetById(issue.ModifiedStaffID.Value) == null)
            {
                return 0;
            }
            
            issue.Status = IssueStatus.Open;
            issue.CreateStaffID = issue.ModifiedStaffID;
            issue.OpenStaffID = issue.ModifiedStaffID;
            issue.OpenedDate = DateTime.Today.Date;
            issue.ModifiedDate = DateTime.Today.Date;
            issue.Stage = IssueStage.Open;
            _issueRepository.Add(issue);

            if (salesCategoryIDs.Count > 0)
            {
                List<IssueCategoryMapping> categories = new List<IssueCategoryMapping>();
                foreach (int eachId in salesCategoryIDs)
                {
                    SalesCategory foundCategory = _salesCategoryRepository.GetById(eachId);
                    if (foundCategory == null)
                    {
                        return 0;
                    }
                    categories.Add(new IssueCategoryMapping { SalesCategoryID = eachId, IsDeleted = false });
                }
                issue.IssueCategoryMappings = categories;
            }

            _unitOfWork.Commit();
            return issue.ID;
        }

        public IEnumerable<Issue> GetAllIssues()
        {
            return _issueRepository.GetAll();
        }

        public IEnumerable<Issue> GetByCustomer(int customerID)
        {
            var foundCustomer = _customerRepository.GetById(customerID);
            if (foundCustomer != null)
            {
                var issues = foundCustomer.Issues;
                if (issues.Any())
                {
                    return issues;
                }
            }

            return null;
        }

        public List<string> GetStages()
        {
            return new List<string>
            {
                IssueStage.Open,
                IssueStage.Solving,
                IssueStage.Closed
            };
        }
        public List<string> GetStatus()
        {
            return new List<string>
            {
                IssueStatus.Open,
                IssueStatus.Doing,
                IssueStatus.Overdue,
                IssueStatus.Done,
                IssueStatus.Failed
            };
        }
    }
}
