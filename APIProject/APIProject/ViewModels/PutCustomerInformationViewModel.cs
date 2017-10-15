using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PutCustomerInformationViewModel
    {
        [Required]
        public int CustomerID { get; set; }
        [Required]
        public string CutomerType { get; set; }


        public Customer ToCustomerModel()
        {
            Customer customer = new Customer();
            customer.ID = this.CustomerID;
            customer.CustomerType = this.CutomerType;

            return customer;
        }

    }
}