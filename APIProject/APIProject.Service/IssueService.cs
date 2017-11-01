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
        IEnumerable<Issue> GetAll();
        Issue Get(int id);
        Issue Add(Issue issue);
        void UpdateInfo(Issue issue);
        void SetSolve(Issue issue);
        void SetDone(Issue issue);
        void SetFail(Issue issue);
        void SaveChanges();
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
            //if (_staffRepository.GetById(issue.ModifiedStaffID.Value) == null)
            //{
            //    return 0;
            //}

            //issue.CustomerID = foundCustomer.ID;
            //issue.Status = IssueStatus.Open;
            //issue.CreateStaffID = issue.ModifiedStaffID;
            //issue.OpenStaffID = issue.ModifiedStaffID;
            //issue.OpenedDate = DateTime.Today.Date;
            //issue.ModifiedDate = DateTime.Today.Date;
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
                    categories.Add(new IssueCategoryMapping { SalesCategoryID = eachId, IsDelete = false });
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
            var entities = _issueRepository.GetAll()
                .Where(c => c.CustomerID == customerID && c.IsDelete == false);
            return entities;
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
        public IEnumerable<Issue> GetAll()
        {
            var entities = _issueRepository.GetAll().Where(c => c.IsDelete == false);
            return entities;
        }
        public Issue Get(int id)
        {
            var entity = _issueRepository.GetById(id);
            if(entity != null)
            {
                return entity;
            }
            else
            {
                throw new Exception(CustomError.IssueNotFound);
            }
        }
        public Issue Add(Issue issue)
        {
            var contactCus = _customerRepository.GetByContact(issue.ContactID.Value);
            VerifyCustomerCanAdd(contactCus);
            var entity = new Issue
            {
                ContactID = issue.ContactID.Value,
                CustomerID=contactCus.ID,
                CreateStaffID = issue.CreateStaffID.Value,
                Title = issue.Title,
                Description = issue.Description,
                Status = IssueStatus.Open,
                CreatedDate=DateTime.Now,
                UpdatedDate=DateTime.Now,
            };
            _issueRepository.Add(entity);
            _unitOfWork.Commit();
            return entity;
        }

        public void UpdateInfo(Issue issue)
        {
            var entity = _issueRepository.GetById(issue.ID);
            VerifyCanUpdateInfo(entity);
            entity.Title = issue.Title;
            entity.Description = issue.Description;
            entity.UpdatedDate = DateTime.Now;
            _issueRepository.Update(entity);
        }

        public void SetSolve(Issue issue)
        {
            var entity = _issueRepository.GetById(issue.ID);
            entity.SolveDate = issue.SolveDate;
            entity.Status = IssueStatus.Doing;
            entity.UpdatedDate = DateTime.Now;
            _issueRepository.Update(entity);
        }
        public void SetDone(Issue issue)
        {
            var entity = _issueRepository.GetById(issue.ID);
            VerifyCanSetDone(entity);
            entity.Status = IssueStatus.Done;
            entity.ClosedDate = DateTime.Now;
            entity.UpdatedDate = DateTime.Now;
            _issueRepository.Update(entity);
        }
        public void SetFail(Issue issue)
        {
            var entity = _issueRepository.GetById(issue.ID);
            VerifyCanSetFail(entity);
            entity.Status = IssueStatus.Failed;
            entity.UpdatedDate = DateTime.Now;
            entity.ClosedDate = DateTime.Today;
            _issueRepository.Update(entity);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
        #region private verify
        private void VerifyCustomerCanAdd(Customer customer)
        {
            List<string> requiredCustomerTypes = new List<string>
            {
                CustomerType.Official,
                CustomerType.Inside,
                CustomerType.Outside
            };
            if (!requiredCustomerTypes.Contains(customer.CustomerType))
            {
                throw new Exception(CustomError.CustomerTypeRequired
                    + String.Join(", ", requiredCustomerTypes));
            }
        }
        private void VerifyCanUpdateInfo(Issue issue)
        {
            List<string> requiredStatus = new List<string>
            {
                IssueStatus.Open,
                IssueStatus.Doing,
                IssueStatus.Overdue
            };
            if (!requiredStatus.Contains(issue.Status))
            {
                throw new Exception(CustomError.IssueStatusRequired
                    + String.Join(", ", requiredStatus));
            }
        }
        private void VerifyCanSetDone(Issue issue)
        {
            List<string> requiredStatus = new List<string>
            {
                IssueStatus.Doing,
                IssueStatus.Overdue
            };
            if (!requiredStatus.Contains(issue.Status))
            {
                throw new Exception(CustomError.IssueStatusRequired
                    + String.Join(", ", requiredStatus));
            }
        }
        private void VerifyCanSetFail(Issue issue)
        {
            List<string> requiredStatus = new List<string>
            {
                IssueStatus.Doing,
                IssueStatus.Overdue
            };
            if (!requiredStatus.Contains(issue.Status))
            {
                throw new Exception(CustomError.IssueStatusRequired
                    + String.Join(", ", requiredStatus));
            }
        }
        #endregion
    }
}
