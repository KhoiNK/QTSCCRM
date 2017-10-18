using APIProject.Data.Infrastructure;
using APIProject.Data.Repositories;
using APIProject.GlobalVariables;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace APIProject.Service
{
    public class CustomerService : ICustomerService
    {

        private readonly ICustomerRepository _customerRepository;
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IAppConfigRepository _appConfigRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(ICustomerRepository _customerRepository, IUnitOfWork _unitOfWork,
            IOpportunityRepository _opportunityRepository, IActivityRepository _activityRepository,
            IAppConfigRepository _appConfigRepository, IIssueRepository _issueRepository)
        {
            this._customerRepository = _customerRepository;
            this._opportunityRepository = _opportunityRepository;
            this._activityRepository = _activityRepository;
            this._appConfigRepository = _appConfigRepository;
            this._issueRepository = _issueRepository;
            this._unitOfWork = _unitOfWork;
        }


        public int CreateNewLead(Customer customer)
        {
            _customerRepository.Add(customer);
            customer.CustomerType = CustomerType.Lead;
            _unitOfWork.Commit();
            return customer.ID;
        }

        public bool EditCustomer(Customer customer)
        {
            Customer foundCustomer = _customerRepository.GetById(customer.ID);
            if (foundCustomer == null)
            {
                return false;
            }

            if (foundCustomer.CustomerType == CustomerType.Lead)
            {
                return false;
            }

            foundCustomer.CustomerType = customer.CustomerType;

            _unitOfWork.Commit();

            return true;
        }

        public bool EditLead(Customer customer)
        {
            Customer foundCustomer = _customerRepository.GetById(customer.ID);
            if (foundCustomer == null)
            {
                return false;
            }

            if (foundCustomer.CustomerType != CustomerType.Lead)
            {
                return false;
            }

            foundCustomer.Name = customer.Name;
            foundCustomer.Address = customer.Address;
            foundCustomer.TaxCode = customer.TaxCode;
            foundCustomer.EstablishedDate = customer.EstablishedDate;
            foundCustomer.AvatarSrc = customer.AvatarSrc;


            _unitOfWork.Commit();
            return true;
        }

        public IEnumerable<Customer> GetCustomerList()
        {
            var customers = _customerRepository.GetAll();
            foreach (var customer in customers)
            {
                ConcatImgUri(customer);

            }
            return _customerRepository.GetAll();

        }

        private void ConcatImgUri(Customer customer)
        {
            if (customer.AvatarSrc != null)
            {
                customer.AvatarSrc = _appConfigRepository.GetHost() + "/"
                    + FileDirectory.CustomerAvatarFolder + "/"
                    + customer.AvatarSrc;
            }
        }



        public Customer GetByOpportunity(int opportunityID)
        {
            var foundOpportunity = _opportunityRepository.GetById(opportunityID);
            if (foundOpportunity != null)
            {
                ConcatImgUri(foundOpportunity.Customer);
                return foundOpportunity.Customer;
            }
            return null;
        }

        public Customer GetByActivity(int activityID)
        {
            var foundActivity = _activityRepository.GetById(activityID);
            if (foundActivity != null)
            {
                ConcatImgUri(foundActivity.Customer);
                return foundActivity.Customer;
            }
            return null;
        }

        public List<string> GetCustomerTypes()
        {
            return new List<string>
            {
                CustomerType.Lead,
                CustomerType.Official,
                CustomerType.Inside,
                CustomerType.Outside,
            };
        }

        public Customer GetByIssue(int issueID)
        {
            var foundIssue = _issueRepository.GetById(issueID);
            if (foundIssue != null)
            {
                ConcatImgUri(foundIssue.Customer);
                return foundIssue.Customer;
            }
            return null;
        }


    }

    public interface ICustomerService
    {
        int CreateNewLead(Customer customer);
        bool EditCustomer(Customer customer);
        bool EditLead(Customer customer);
        Customer GetByActivity(int activityID);
        Customer GetByOpportunity(int opportunityID);
        IEnumerable<Customer> GetCustomerList();
        List<string> GetCustomerTypes();
        Customer GetByIssue(int issueID);
    }
}
