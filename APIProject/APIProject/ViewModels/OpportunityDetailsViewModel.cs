﻿using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class OpportunityDetailsViewModel
    {
        public OpportunityDetailViewModel OpportunityDetail { get; set; }
        public List<ActivityDetailViewModel> HistoryActivities { get; set; }
        public QuoteViewModel QuoteDetail { get; set; }
        public StaffDetailViewModel StaffDetail { get; set; }
        public CustomerDetailViewModel CustomerDetail { get; set; }
        public ContactViewModel ContactDetail { get; set; }
        public List<int> CategoryIDs { get; set; }
        public OpportunityDetailsViewModel(Opportunity dto)
        {
            OpportunityDetail = new OpportunityDetailViewModel(dto);
            var activities = dto.Activities;
            HistoryActivities = new List<ActivityDetailViewModel>();
            foreach (var activity in activities)
            {
                HistoryActivities.Add(new ActivityDetailViewModel(activity));
            }
            //var lastQuote = dto.Quotes.Where(c => c.IsDeleted == false).SingleOrDefault();
            var lastQuote = dto.Quotes.Where(c => c.IsDelete == false).OrderByDescending(c => c.ID).FirstOrDefault();
            if (lastQuote != null)
            {
                QuoteDetail = new QuoteViewModel(lastQuote);
            }
            StaffDetail = new StaffDetailViewModel(dto.CreatedStaff);
            CustomerDetail = new CustomerDetailViewModel(dto.Customer);
            ContactDetail = new ContactViewModel(dto.Contact);
            CategoryIDs = dto.OpportunityCategoryMappings.Where(c=>c.IsDeleted==false).Select(c => c.SalesCategoryID).ToList();

        }
    }
}