﻿using APIProject.Data.Infrastructure;
using APIProject.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIProject.Model.Models;
using System.IO;
using System.Web;
using APIProject.GlobalVariables;

namespace APIProject.Service
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly IAppConfigRepository _appConfigRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ContactService(IContactRepository _contactRepository,
            ICustomerRepository _customerRepository, IUnitOfWork _unitOfWork,
            IOpportunityRepository _opportunityRepository,
            IActivityRepository _activityRepository,
            IAppConfigRepository _appConfigRepository,
            IIssueRepository _issueRepository)
        {
            this._contactRepository = _contactRepository;
            this._customerRepository = _customerRepository;
            this._opportunityRepository = _opportunityRepository;
            this._activityRepository = _activityRepository;
            this._appConfigRepository = _appConfigRepository;
            this._issueRepository = _issueRepository;
            this._unitOfWork = _unitOfWork;
        }

        public Contact Add(Contact contact)
        {
            var entity = new Contact
            {
                CustomerID=contact.CustomerID,
                Name=contact.Name,
                Position=contact.Position,
                Email=contact.Email,
                Phone=contact.Phone,
                AvatarSrc=contact.AvatarSrc,
                CreatedDate=DateTime.Now,
                UpdatedDate=DateTime.Now
            };
            _contactRepository.Add(entity);
            _unitOfWork.Commit();
            return entity;
        }
        public void UpdateInfo(Contact contact)
        {
            var entity = _contactRepository.GetById(contact.ID);
            entity.Position = contact.Position;
            entity.Phone = contact.Phone;
            entity.Email = contact.Email;
            entity.AvatarSrc = contact.AvatarSrc;
            entity.UpdatedDate = DateTime.Now;
            _contactRepository.Update(entity);
        }
        public Contact GetContactByOpportunity(int opportunityID)
        {
            var foundOpportunity = _opportunityRepository.GetById(opportunityID);
            if (foundOpportunity != null)
            {
                return foundOpportunity.Contact;
            }
            return null;
        }
        public Contact GetContactByActivity(int activityID)
        {
            var foundActivity = _activityRepository.GetById(activityID); 
            if (foundActivity != null)
            {
                return foundActivity.Contact;
            }
            return null;
        }
        public Contact GetByIssue(int issueID)
        {
            var foundIssue = _issueRepository.GetById(issueID);//todo
            if (foundIssue != null)
            {
                return foundIssue.Contact;
            }
            return null;
        }
        public Contact Get(int id)
        {
            var entity = _contactRepository.GetById(id);
            if (entity != null)
            {
                return entity;
            }
            else
            {
                throw new Exception(CustomError.ContactNotFound);
            }
        }
        public IEnumerable<Contact> GetByCustomer(int customerID)
        {
            var entities = _contactRepository.GetAll().Where(
                c => c.CustomerID == customerID&&c.IsDelete==false);
            return entities;
        }

    }

    public interface IContactService
    {
        Contact GetByIssue(int issueID);
        Contact GetContactByActivity(int activityID);
        Contact GetContactByOpportunity(int opportunityID);
        Contact Get(int id);
        Contact Add(Contact contact);
        void UpdateInfo(Contact contact);
        IEnumerable<Contact> GetByCustomer(int customerID);

    }
}
