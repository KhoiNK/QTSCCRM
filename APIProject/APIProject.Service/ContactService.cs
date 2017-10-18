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

        public int CreateContact(Contact contact)
        {
            Customer foundCustomer = _customerRepository.GetById(contact.CustomerID);
            if (foundCustomer == null)
            {
                return 0;
            }

            _contactRepository.Add(contact);
            //_unitOfWork.Commit();

            //if (avatarB64 != null)
            //{
            //    contact.AvatarSrc = InsertContactAvatar(contact.ID, avatarName, avatarB64);
            //}

            _unitOfWork.Commit();

            return contact.ID;
        }

        private string InsertContactAvatar(int contactID, string avatarName, string avatarB64)
        {
            string fileExtension = Path.GetExtension(avatarName);
            string fileRoot = HttpContext.Current.Server.MapPath("~/ContactAvatarFiles");
            string fileName = contactID + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day
                + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + fileExtension;
            string filePath = Path.Combine(fileRoot, fileName);
            try
            {
                File.WriteAllBytes(filePath, Convert.FromBase64String(avatarB64));
            }
            catch (FormatException e)
            {
                return null;
            }
            return filePath;
        }
        public bool EditContact(Contact contact)
        {
            Contact foundContact = _contactRepository.GetById(contact.ID);
            if (foundContact == null)
            {
                return false;
            }

            foundContact.Name = contact.Name;
            foundContact.Position = contact.Position;
            foundContact.Phone = contact.Phone;
            foundContact.Email = contact.Email;
            foundContact.AvatarSrc = contact.AvatarSrc;

            _unitOfWork.Commit();
            return true;
        }

        public Contact GetContactByOpportunity(int opportunityID)
        {
            var foundOpportunity = _opportunityRepository.GetById(opportunityID);
            if (foundOpportunity != null)
            {
                ConcatImgUri(foundOpportunity.Contact);
                return foundOpportunity.Contact;
            }
            return null;
        }

        public Contact GetContactByActivity(int activityID)
        {
            var foundActivity = _activityRepository.GetById(activityID); 
            if (foundActivity != null)
            {
                ConcatImgUri(foundActivity.Contact);
                return foundActivity.Contact;
            }
            return null;
        }
        public IEnumerable<Contact> GetByCustomer(int customerID)
        {
            var foundCustomer = _customerRepository.GetById(customerID);
            if (foundCustomer != null)
            {
                var contacts = foundCustomer.Contacts;
                if (contacts.Any())
                {
                    foreach (var contact in contacts)
                    {
                        ConcatImgUri(contact);
                    }

                    return contacts;
                }
            }

            return null;
        }

        private void ConcatImgUri(Contact contact)
        {
            if (contact.AvatarSrc != null)
            {
                contact.AvatarSrc = _appConfigRepository.GetHost() + "/"
                    + FileDirectory.ContactAvatarFolder + "/"
                    + contact.AvatarSrc;
            }
        }

        public Contact GetByIssue(int issueID)
        {
            var foundIssue = _issueRepository.GetById(issueID);//todo
            if (foundIssue != null)
            {
                ConcatImgUri(foundIssue.Contact);
                return foundIssue.Contact;
            }
            return null;
        }


    }

    public interface IContactService
    {
        int CreateContact(Contact contact);
        bool EditContact(Contact contact);
        IEnumerable<Contact> GetByCustomer(int customerID);
        Contact GetByIssue(int issueID);
        Contact GetContactByActivity(int activityID);
        Contact GetContactByOpportunity(int opportunityID);
    }
}
