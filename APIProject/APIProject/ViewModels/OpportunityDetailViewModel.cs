using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class OpportunityDetailViewModel
    {
        public int ID { get; set; }
        public string StageName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //QuoteViewModel Quote { get; set; }
        //List<ActivityViewModel> HistoryActivities { get; set; }
        //CustomerDetailViewModel Customer { get; set; }
        //ContactViewModel Contact { get; set; }
        

        public OpportunityDetailViewModel(Opportunity dto)
        {
            this.ID = dto.ID;
            this.StageName = dto.StageName;
            this.Title = dto.Title;
            this.Description = dto.Description;
            //if(dto.Activities.Count > 0)
            //{
            //    HistoryActivities = new List<ActivityViewModel>();
            //    foreach(Activity item in dto.Activities)
            //    {
            //        HistoryActivities.Add(new ActivityViewModel(item));
            //    }
            //}
            //if(dto.Quotes.LastOrDefault() != null)
            //{
            //    Quote = new QuoteViewModel(dto.Quotes.LastOrDefault());
            //}
        }
    }

    
    
}