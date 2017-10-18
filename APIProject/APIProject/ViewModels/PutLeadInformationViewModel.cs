using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PutLeadInformationViewModel
    {
        [Required]
        public int CustomerID { get; set; }
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
            _customer.ID = this.CustomerID;
            _customer.Name = this.Name;
            _customer.Address = this.Address;
            _customer.EstablishedDate = this.EstablishedDate;
            _customer.TaxCode = this.TaxCode;
            _customer.AvatarSrc = this.Avatar.Name;
            return _customer;
        }
    }
}