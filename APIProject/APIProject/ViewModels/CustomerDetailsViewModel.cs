using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class CustomerDetailsViewModel
    {
        public CustomerDetailViewModel CustomerDetail { get; set; }
        public List<ContactViewModel> Contacts { get; set; }
        public List<IssueDetailViewModel> Issues { get; set; }
        public List<OpportunityDetailViewModel> Opportunities { get; set; }
        public List<ActivityDetailViewModel> Activities { get; set; }
        public CustomerDetailsViewModel(Customer dto)
        {
            CustomerDetail = new CustomerDetailViewModel(dto);
            if (dto.Contacts.Any())
            {
                Contacts = new List<ContactViewModel>();
                foreach (var contact in dto.Contacts)
                {
                    Contacts.Add(new ContactViewModel(contact));
                }
            }
            else
            {
                Contacts = null;
            }
            if (dto.Issues.Any())
            {
                Issues = new List<IssueDetailViewModel>();
                foreach (var issue in dto.Issues)
                {
                    Issues.Add(new IssueDetailViewModel(issue));
                }
            }
            else
            {
                Issues = null;
            }
            if (dto.Opportunities.Any())
            {
                Opportunities = new List<OpportunityDetailViewModel>();
                foreach (var opportunity in dto.Opportunities)
                {
                    Opportunities.Add(new OpportunityDetailViewModel(opportunity));
                }
            }
            else
            {
                Opportunities = null;
            }
            if (dto.Activities.Any())
            {
                Activities = new List<ActivityDetailViewModel>();
                foreach (var activity in dto.Activities)
                {
                    Activities.Add(new ActivityDetailViewModel(activity));
                }
            }
            else
            {
                Activities = null;
            }
        }
    }
}