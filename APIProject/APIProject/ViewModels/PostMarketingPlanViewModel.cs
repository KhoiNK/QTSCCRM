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
        public CustomBase64File? EventScheduleFile { get; set; }
        public CustomBase64File? TaskAssignFile { get; set; }
        public CustomBase64File? BudgetFile { get; set; }
        public CustomBase64File? LicenseFile { get; set; }
        [Required]
        public bool IsFinished { get; set; }

        public MarketingPlan ToMarketingPlanModel(string fileRoot)
        {
            MarketingPlan _plan = new MarketingPlan();
            _plan.ModifiedStaffID = this.StaffID;
            _plan.Title = this.Title;
            _plan.Budget = this.Budget;
            _plan.Description = this.Description;
            _plan.StartDate = this.StartDate;
            _plan.EndDate = this.EndDate;
            if (BudgetFile.HasValue)
            {
                _plan.BudgetFileSrc = fileRoot + $@"\\{BudgetFile.Value.Name}";

            }
            if (EventScheduleFile.HasValue)
            {
                _plan.EventScheduleFileSrc = fileRoot + $@"\\{EventScheduleFile.Value.Name}";

            }
            if (TaskAssignFile.HasValue)
            {
                _plan.EventScheduleFileSrc = fileRoot + $@"\\{TaskAssignFile.Value.Name}";

            }
            if (LicenseFile.HasValue)
            {
                _plan.LicenseFileSrc = fileRoot + $@"\\{LicenseFile.Value.Name}";

            }
            return _plan;

        }
    }

    public struct CustomBase64File
    {
        public string Name { get; set; }
        [Required]
        public string Base64Content { get; set; }
    }


}