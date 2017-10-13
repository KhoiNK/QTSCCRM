using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PostMarketingPlanViewModel
    {
        [Required]
        public int StaffID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int Budget { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public CustomBase64FileViewModel EventScheduleFile { get; set; }
        public CustomBase64FileViewModel TaskAssignFile { get; set; }
        public CustomBase64FileViewModel BudgetFile { get; set; }
        public CustomBase64FileViewModel LicenseFile { get; set; }
        [Required]
        public bool IsFinished { get; set; }

        public MarketingPlan ToMarketingPlanModel()
        {
            MarketingPlan _plan = new MarketingPlan();
            _plan.ModifiedStaffID = this.StaffID;
            _plan.Title = this.Title;
            _plan.Budget = this.Budget;
            _plan.Description = this.Description;
            _plan.StartDate = this.StartDate;
            _plan.EndDate = this.EndDate;
            return _plan;

        }
    }

    //public struct CustomBase64File
    //{
    //    public string Name { get; set; }
    //    [Required]
    //    public string Base64Content { get; set; }
    //}


}