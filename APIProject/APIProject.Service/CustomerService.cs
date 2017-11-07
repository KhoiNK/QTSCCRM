﻿using APIProject.Data.Infrastructure;
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

        public void UpdateType(Customer customer)
        {
            var entity = _customerRepository.GetById(customer.ID);
            VerifyCanUpdateType(entity);
            entity.CustomerType = customer.CustomerType;
            _customerRepository.Update(entity);
        }

        public void UpdateInfo(Customer customer)
        {
            var entity = _customerRepository.GetById(customer.ID);
            VerifyCanUpdateInfo(entity);
            entity.Name = customer.Name;
            entity.Address = customer.Address;
            entity.TaxCode = customer.TaxCode;
            entity.EstablishedDate = customer.EstablishedDate;
            entity.AvatarSrc = customer.AvatarSrc;
            _customerRepository.Update(entity);
        }

        public IEnumerable<Customer> GetCustomerList()
        {
            return _customerRepository.GetAll().Where(c=>c.IsDelete==false);
        }

        public Customer GetByActivity(int activityID)
        {
            var foundActivity = _activityRepository.GetById(activityID);
            if (foundActivity != null)
            {
                return foundActivity.Customer;
            }
            return null;
        }

        public List<string> GetCustomerTypes()
        {
            return CustomerType.GetList();
        }

        public Customer GetByIssue(int issueID)
        {
            var foundIssue = _issueRepository.GetById(issueID);
            if (foundIssue != null)
            {
                return foundIssue.Customer;
            }
            return null;
        }
        
        public Customer Get(int id)
        {
            return _customerRepository.GetById(id);
        }
        public IEnumerable<Customer> GetAll()
        {
            return _customerRepository.GetAll();
        }
        public IEnumerable<Customer> GetOfficial()
        {
            List<string> requiredCustomerTypes = new List<string>
            {
                CustomerType.Official,
                CustomerType.Inside,
                CustomerType.Outside
            };
            var entities = _customerRepository.GetAll().Where(c => c.IsDelete == false
            && requiredCustomerTypes.Contains(c.CustomerType));
            return entities;
        }
        public Customer GetByOpportunity(int opportunityID)
        {
            var foundOpportunity = _opportunityRepository.GetById(opportunityID);
            var customerID = _opportunityRepository.GetById(opportunityID).CustomerID;
            var oppCus = _customerRepository.GetById(customerID.Value);
            return oppCus;
        }
        public Customer Add(Customer customer)
        {
            VerifyCanAdd(customer);
            var entity = new Customer
            {
                Name = customer.Name,
                Address = customer.Address,
                EstablishedDate = customer.EstablishedDate,
                TaxCode = customer.TaxCode,
                AvatarSrc = customer.AvatarSrc,
                CustomerType = CustomerType.Lead,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            _customerRepository.Add(entity);
            _unitOfWork.Commit();
            return entity;
        }

        public void Update(Customer customer)
        {
            var entity = _customerRepository.GetById(customer.ID);
            entity = customer;
            entity.UpdatedDate = DateTime.Now;
            _customerRepository.Update(entity);
        }
        public void ConvertToCustomer(Customer customer)
        {
            var entity = _customerRepository.GetById(customer.ID);
            VerifyCanConvert(entity);
            entity.CustomerType = CustomerType.Official;
            entity.ConvertedDate = DateTime.Today.Date;
            entity.UpdatedDate = DateTime.Now;
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        #region private verify
        private void VerifyCanAdd(Customer customer)
        {
            var existedCustomer = _customerRepository.GetAll().Where(c => c.TaxCode == customer.TaxCode &&
            c.IsDelete == false).FirstOrDefault();
            if (existedCustomer != null)
            {
                throw new Exception(CustomError.TaxCodeIsUsed);
            }
        }
        private void VerifyCanConvert(Customer cus)
        {
            if (cus.CustomerType != CustomerType.Lead)
            {
                throw new Exception(CustomError.CustomerTypeRequired
                    + CustomerType.Lead);
            }
        }
        private void VerifyCanUpdateInfo(Customer customer)
        {
            if(customer.CustomerType!= CustomerType.Lead)
            {
                throw new Exception(CustomError.CustomerTypeRequired
                    + CustomerType.Lead);
            }
        }
        private void VerifyCanUpdateType(Customer customer)
        {
            List<string> requiredType = new List<string>
            {
                CustomerType.Official,
                CustomerType.Inside,
                CustomerType.Outside
            };
            if (!requiredType.Contains(customer.CustomerType))
            {
                throw new Exception(CustomError.CustomerTypeRequired
                    + String.Join(", ", requiredType));
            }
        }
#endregion
    }

    public interface ICustomerService
    {
        int CreateNewLead(Customer customer);
        Customer GetByActivity(int activityID);
        IEnumerable<Customer> GetCustomerList();
        List<string> GetCustomerTypes();
        Customer GetByIssue(int issueID);
        Customer Get(int id);
        IEnumerable<Customer> GetAll();
        IEnumerable<Customer> GetOfficial();
        Customer GetByOpportunity(int opportunityID);
        Customer Add(Customer customer);
        void UpdateInfo(Customer customer);
        void UpdateType(Customer customer);
        void Update(Customer customer);
        void ConvertToCustomer(Customer customer);
        void SaveChanges();
    }
}
