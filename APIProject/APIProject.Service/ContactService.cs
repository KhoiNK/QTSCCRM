using APIProject.Data.Infrastructure;
using APIProject.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIProject.Model.Models;
using System.IO;
using System.Web;

namespace APIProject.Service
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ContactService(IContactRepository _contactRepository,
            ICustomerRepository _customerRepository, IUnitOfWork _unitOfWork)
        {
            this._contactRepository = _contactRepository;
            this._customerRepository = _customerRepository;
            this._unitOfWork = _unitOfWork;
        }

        public int CreateContact(Contact contact, string avatarName, string avatarB64)
        {
            Customer foundCustomer = _customerRepository.GetById(contact.CustomerID);
            if (foundCustomer == null)
            {
                return 0;
            }

            _contactRepository.Add(contact);
            _unitOfWork.Commit();

            if (avatarB64 != null)
            {
                contact.AvatarSrc = InsertContactAvatar(contact.ID, avatarName, avatarB64);
            }

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
        public bool EditContact(Contact contact, string avatarName, string avatarB64)
        {
            //Customer foundCustomer = _customerRepository.GetById(contact.CustomerID);
            //if (foundCustomer == null)
            //{
            //    return false;
            //}
            Contact foundContact = _contactRepository.GetById(contact.ID);
            if (foundContact == null)
            {
                return false;
            }

            foundContact.Name = contact.Name;
            foundContact.Position = contact.Position;
            foundContact.Phone = contact.Phone;
            foundContact.Email = contact.Email;
            if (avatarB64 != null)
            {
                foundContact.AvatarSrc = InsertContactAvatar(foundContact.ID, avatarName, avatarB64);
            }

            _unitOfWork.Commit();
            return true;
        }


    }

    public interface IContactService
    {
        int CreateContact(Contact contact, string avatarName, string avatarB64);
        bool EditContact(Contact contact, string avatarName, string avatarB64);
    }
}
