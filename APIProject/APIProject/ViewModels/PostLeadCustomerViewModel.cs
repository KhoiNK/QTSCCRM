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
        public CustomBase64FileViewModel Avatar { get; set; }

        public Customer ToCustomerModel()
        {
            Customer _customer = new Customer();
            _customer.Name = this.Name;
            _customer.Address = this.Address;
            _customer.EstablishedDate = this.EstablishedDate;
            _customer.TaxCode = this.TaxCode;

            return _customer;
        }
    }
}