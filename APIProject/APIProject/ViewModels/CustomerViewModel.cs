using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class CustomerViewModel
    {
        public int ID { get; set; }
        public string AvatarUrl { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime EstablishedDate { get; set; }
        public string CustomerType { get; set; }

        public CustomerViewModel(Customer customer)
        {
            ID = customer.ID;
            AvatarUrl = customer.AvatarSrc;
            Name = customer.Name;
            Address = customer.Address;
            EstablishedDate = customer.EstablishedDate;
            CustomerType = customer.CustomerType;
        }
    }
}