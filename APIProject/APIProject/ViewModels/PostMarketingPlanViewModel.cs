using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PostMarketingPlanViewModel
    {
        public int StaffID { get; set; }
        public string Title { get; set; }
        public int Budget { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsFinished { get; set; }

        public MarketingPlan ToMarketingPlanModel()
        {
            return new MarketingPlan
            {
                ModifiedStaffID = this.StaffID,
                Title = this.Title,
                Budget = this.Budget,
                Description = this.Description,
                StartDate = this.StartDate,
                EndDate = this.EndDate
            };
        }
    }
}