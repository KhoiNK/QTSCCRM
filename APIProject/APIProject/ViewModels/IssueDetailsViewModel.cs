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
        public List<string> CategoryNames { get; set; }
        public IssueDetailsViewModel(Issue dto)
        {
            IssueDetail = new IssueDetailViewModel(dto);
            CustomerDetail = new CustomerDetailViewModel(dto.Customer);
            ContactDetail = new ContactViewModel(dto.Contact);
            if (dto.IssueCategoryMappings.Any())
            {
                //CategoryIDs = dto.IssueCategoryMappings.Select(c => c.SalesCategoryID).ToList();
                CategoryNames = dto.IssueCategoryMappings.Select(c => c.SalesCategory.Name).ToList();
            }
        }
    }
}