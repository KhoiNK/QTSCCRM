using APIProject.Data.Infrastructure;
using APIProject.Data.Repositories;
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
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(ICustomerRepository _customerRepository, IUnitOfWork _unitOfWork)
        {
            this._customerRepository = _customerRepository;
            this._unitOfWork = _unitOfWork;
        }
        private string InsertCustomerAvatar(int customerID, string avatarName, string b64Content)
        {
            string fileExtension = Path.GetExtension(avatarName);
            string fileRoot = HttpContext.Current.Server.MapPath("~/CustomerAvatarFiles");
            string fileName = customerID + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day
                + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + fileExtension;
            string filePath = Path.Combine(fileRoot, fileName);
            try
            {
                File.WriteAllBytes(filePath, Convert.FromBase64String(b64Content));
            }
            catch (FormatException)
            {
                return null;
            }
            return filePath;
        }
        public int CreateNewLead(Customer customer,string avatarName, string avatarB64)
        {
            _customerRepository.Add(customer);
            customer.IsLead = true;
            _unitOfWork.Commit();

            if(avatarB64 != null)
            {
                customer.AvatarSrc = InsertCustomerAvatar(customer.ID, avatarName ,avatarB64);
            }
            _unitOfWork.Commit();
            return customer.ID;
        }

        public bool EditCustomer(Customer customer)
        {
            Customer foundCustomer = _customerRepository.GetById(customer.ID);
            if(foundCustomer == null)
            {
                return false;
            }

            if (foundCustomer.IsLead)
            {
                return false;
            }

            foundCustomer.CustomerType = customer.CustomerType;

            _unitOfWork.Commit();

            return true;
        }

        public bool EditLead(Customer customer, string avatarName, string avatarB64)
        {
            Customer foundCustomer = _customerRepository.GetById(customer.ID);
            if(foundCustomer == null)
            {
                return false;
            }

            if (!foundCustomer.IsLead)
            {
                return false;
            }

            foundCustomer.Name = customer.Name;
            foundCustomer.Address = customer.Address;
            foundCustomer.TaxCode = customer.TaxCode;
            foundCustomer.EstablishedDate = customer.EstablishedDate;
            if (avatarB64 != null)
            {
                foundCustomer.AvatarSrc = InsertCustomerAvatar(customer.ID, avatarName, avatarB64);
            }

            _unitOfWork.Commit();
            return true;
        }

        public IEnumerable<Customer> GetCustomerList()
        {
            return _customerRepository.GetAll();
        }
    }

    public interface ICustomerService
    {
        int CreateNewLead(Customer customer, string avatarName, string avatarB64);
        bool EditCustomer(Customer customer);
        bool EditLead(Customer customer, string avatarName, string avatarB64);
        IEnumerable<Customer> GetCustomerList();
    }
}
