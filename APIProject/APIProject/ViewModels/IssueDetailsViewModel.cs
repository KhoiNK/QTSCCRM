using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class IssueDetailsViewModel
    {
        public IssueDetailViewModel IssueDetail { get; set; }
        public CustomerDetailViewModel CustomerDetail { get; set; }
        public ContactViewModel ContactDetail { get; set; }
        //public List<int> CategoryIDs { get; set; }
        public List<IssueCategoryViewModel> Categories { get; set; }
        public IssueDetailsViewModel(Issue issue, Contact contact, Customer customer,
            List<SalesCategory> issueCategories)
        {
            IssueDetail = new IssueDetailViewModel(issue);
            CustomerDetail = new CustomerDetailViewModel(customer);
            ContactDetail = new ContactViewModel(contact);
            Categories = new List<IssueCategoryViewModel>();
            foreach (var category in issueCategories)
            {
                Categories.Add(new IssueCategoryViewModel(category));
            }
        }
    }
    public class IssueCategoryViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IssueCategoryViewModel(SalesCategory dto)
        {
            ID = dto.ID;
            Name = dto.Name;
        }
    }
}