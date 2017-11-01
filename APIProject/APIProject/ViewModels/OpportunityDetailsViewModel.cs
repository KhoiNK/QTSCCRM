using APIProject.Model.Models;
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
        public OpportunityDetailsViewModel(Opportunity oppDto,
            List<Activity> activityDtos,
            Quote quoteDto,
            Staff staffDto,
            Customer cusDto,
            Contact contactDto,
            List<SalesCategory> categoryDtos
            )
        {
            OpportunityDetail = new OpportunityDetailViewModel(oppDto);
            HistoryActivities = activityDtos.Select(c => new ActivityDetailViewModel(c)).ToList();
            //var lastQuote = dto.Quotes.Where(c => c.IsDeleted == false).SingleOrDefault();
            if (quoteDto != null)
            {
                QuoteDetail = new QuoteViewModel(quoteDto);
            }
            StaffDetail = new StaffDetailViewModel(staffDto);
            CustomerDetail = new CustomerDetailViewModel(cusDto);
            ContactDetail = new ContactViewModel(contactDto);
            CategoryIDs = categoryDtos.Select(c => c.ID).ToList();
        }
    }
}