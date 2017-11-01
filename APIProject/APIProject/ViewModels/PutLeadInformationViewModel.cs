using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PutCustomerViewModel
    {
        public bool CustomerUpdated { get; set; }
        public bool CustomerImageUpdated { get; set; }
    }
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
            Customer _customer = new Customer
            {
                ID = this.CustomerID,
                Name = this.Name,
                Address = this.Address,
                EstablishedDate = this.EstablishedDate,
                TaxCode = this.TaxCode,
            };
            if (Avatar != null)
            {
                _customer.AvatarSrc = this.Avatar.Name;
            }
            return _customer;
        }
    }
}