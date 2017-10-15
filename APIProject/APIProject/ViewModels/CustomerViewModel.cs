using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class CustomerViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime EstablishedDate { get; set; }
        public string TaxCode { get; set; }
        public bool IsLead { get; set; }
        public string CustomerType { get; set; }
        public string AvatarSrc { get; set; }

        public CustomerViewModel(Customer customer)
        {
            ID = customer.ID;
            Name = customer.Name;
            Address = customer.Address;
            EstablishedDate = customer.EstablishedDate;
            TaxCode = customer.TaxCode;
            IsLead = customer.IsLead;
            CustomerType = customer.CustomerType;
            AvatarSrc = customer.AvatarSrc;
        }
    }
}