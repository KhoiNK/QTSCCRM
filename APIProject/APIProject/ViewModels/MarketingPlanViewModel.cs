using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class MarketingPlanViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        //public int Budget { get; set; }
        //public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Stage { get; set; }
        public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedStaffName { get; set; }
        public string Status { get; set; }


        public MarketingPlanViewModel(MarketingPlan plan)
        {
            ID = plan.ID;
            Title = plan.Title;
            CreatedDate = plan.CreatedDate;
            Stage = plan.Stage;
            StartDate = plan.StartDate;
            LastModifiedDate = plan.LastModifiedDate;
            LastModifiedStaffName = plan.ModifiedStaff.Name;
            Status = plan.Status;
        }
    }
}