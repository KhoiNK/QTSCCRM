using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class CustomerDetailViewModel
    {
        public int ID { get; set; }
        public string AvatarUrl { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime? EstablishedDate { get; set; }
        public string TaxCode { get; set; }
        public string CustomerType { get; set; }

        public CustomerDetailViewModel(Customer customer)
        {
            ID = customer.ID;
            Name = customer.Name;
            Address = customer.Address;
            EstablishedDate = customer.EstablishedDate;
            TaxCode = customer.TaxCode;
            CustomerType = customer.CustomerType;
            AvatarUrl = customer.AvatarSrc;
        }
    }

    
}