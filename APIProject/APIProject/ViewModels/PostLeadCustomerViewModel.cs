using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PostLeadCustomerViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public DateTime EstablishedDate { get; set; }
        [Required]
        public string TaxCode { get; set; }
        public CustomB64ImageFileViewModel Avatar { get; set; }

        public Customer ToCustomerModel()
        {
            Customer _customer = new Customer();
            _customer.Name = this.Name;
            _customer.Address = this.Address;
            _customer.EstablishedDate = this.EstablishedDate;
            _customer.TaxCode = this.TaxCode;
            if (Avatar != null)
            {
                _customer.AvatarSrc = this.Avatar.Name;
            }
            return _customer;
        }
    }

    public class PostLeadCustomerResponseViewModel
    {
        public bool CustomerAvatarUpdated { get; set; }
        public bool CustomerCreated { get; set; }
        public int CustomerID { get; set; }
    }

    public class PostMarketingCustomerAndContactViewModel
    {
        [Required]
        public int ResultID { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string CustomerAddress { get; set; }
        [Required]
        public string ContactName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        public Customer ToCustomerModel()
        {
            return new Customer
            {
                Name=this.CustomerName,
                Address=this.CustomerAddress
            };
        }
        public Contact ToContactModel()
        {
            return new Contact
            {
                Name = this.ContactName,
                Email = this.Email,
                Phone = this.Phone
            };
        }
    }
}