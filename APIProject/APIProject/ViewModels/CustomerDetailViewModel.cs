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
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime EstablishedDate { get; set; }
        public string TaxCode { get; set; }
        public bool IsLead { get; set; }
        public string CustomerType { get; set; }
        public List<ContactViewModel> Contacts { get; set; }
        public string AvatarSrc { get; set; }

        public CustomerDetailViewModel(Customer customer)
        {
            ID = customer.ID;
            Name = customer.Name;
            Address = customer.Address;
            EstablishedDate = customer.EstablishedDate;
            TaxCode = customer.TaxCode;
            IsLead = customer.IsLead;
            CustomerType = customer.CustomerType;
            AvatarSrc = customer.AvatarSrc;
            if(customer.Contacts.Count > 0)
            {
                Contacts = new List<ContactViewModel>();
                foreach(Contact item in customer.Contacts)
                {
                    ContactViewModel _contact = new ContactViewModel();
                    _contact.ID = item.ID;
                    _contact.Name = item.Name;
                    _contact.Position = item.Position;
                    _contact.Phone = item.Phone;
                    _contact.Email = item.Email;
                    _contact.AvatarSrc = item.AvatarSrc;
                    Contacts.Add(_contact);
                }
            }
        }
    }

    public class ContactViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string AvatarSrc { get; set; }
    }
}