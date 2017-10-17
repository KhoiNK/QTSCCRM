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
        public DateTime EstablishedDate { get; set; }
        public string TaxCode { get; set; }
        public bool IsLead { get; set; }
        public string CustomerType { get; set; }
        public List<ContactViewModel> Contacts { get; set; }


        public CustomerDetailViewModel(Customer customer)
        {
            ID = customer.ID;
            Name = customer.Name;
            Address = customer.Address;
            EstablishedDate = customer.EstablishedDate;
            TaxCode = customer.TaxCode;
            IsLead = customer.IsLead;
            CustomerType = customer.CustomerType;
            AvatarUrl = customer.AvatarSrc;
            if(customer.Contacts.Count > 0)
            {
                Contacts = new List<ContactViewModel>();
                foreach(Contact item in customer.Contacts)
                {
                    ContactViewModel _contact = new ContactViewModel(item);
                    Contacts.Add(_contact);
                }
            }
        }
    }

    public class ContactViewModel
    {
        public int ID { get; set; }
        public string AvatarUrl { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public ContactViewModel(Contact dto)
        {
            this.ID = dto.ID;
            this.Name = dto.Name;
            this.Position = dto.Position;
            this.Phone = dto.Phone;
            this.Email = dto.Email;
            this.AvatarUrl = dto.AvatarSrc;
        }
    }
}