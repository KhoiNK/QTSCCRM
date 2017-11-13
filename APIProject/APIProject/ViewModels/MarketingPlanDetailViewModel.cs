using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class MarketingPlanDetailViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CreatedStaffID { get; set; }
        public string CreatedStaffName { get; set; }

        public string Status { get; set; }

        public MarketingPlanDetailViewModel(MarketingPlan plan)
        {
            ID = plan.ID;
            if (plan.CreateStaffID.HasValue)
            {
                CreatedStaffID = plan.CreateStaffID.Value;
                CreatedStaffName = plan.CreateStaff.Name;
            }
            Title = plan.Title;
            StartDate = plan.StartDate;
            EndDate = plan.EndDate;
            Description = plan.Description;
            Status = plan.Status;

        }
    }
}