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
        public int Budget { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Stage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string ModifiedStaffName { get; set; }
        public int CreatedStaffID { get; set; }
        public string CreatedStaffName { get; set; }
        public int? ValidateStaffID { get; set; }
        public string ValidateStaffName { get; set; }

        public int? AcceptStaffID { get; set; }
        public string AcceptStaffName { get; set; }
        public string ValidateNotes { get; set; }
        public string AcceptNotes { get; set; }
        public int ModifiedStaffID { get; set; }
        public string Status { get; set; }

        public MarketingPlanDetailViewModel(MarketingPlan plan)
        {
            ID = plan.ID;
            if (plan.CreateStaffID.HasValue)
            {
                CreatedStaffID = plan.CreateStaffID.Value;
                CreatedStaffName = plan.CreateStaff.Name;
                CreatedDate = plan.CreatedDate;
            }
            if (plan.ModifiedStaffID.HasValue)
            {
                ModifiedStaffID = plan.ModifiedStaffID.Value;
                ModifiedStaffName = plan.ModifiedStaff.Name;
                LastModifiedDate = plan.LastModifiedDate;
            }
            if (plan.ValidateStaffID.HasValue)
            {
                ValidateStaffID = plan.ValidateStaffID.Value;
                ValidateStaffName = plan.ValidateStaff.Name;
            }
            if (plan.AcceptStaffID.HasValue)
            {
                AcceptStaffID = plan.AcceptStaffID;
                AcceptStaffName = plan.AcceptStaff.Name;
            }
            Title = plan.Title;
            AcceptNotes = plan.AcceptNotes;
            ValidateNotes = plan.ValidateNotes;
            StartDate = plan.StartDate;
            EndDate = plan.EndDate;
            Budget = plan.Budget;
            Description = plan.Description;
            Stage = plan.Stage;
            Status = plan.Status;

        }
    }
}