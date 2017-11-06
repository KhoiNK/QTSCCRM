using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PutMarketingPlanViewModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int StaffID { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public MarketingPlan ToMarketingPlanViewModel()
        {
            return new MarketingPlan
            {
                ID = this.ID,
                Title=this.Title,
                Description=this.Description,
                StartDate=this.StartDate,
                EndDate=this.EndDate
            };
        }
    }
    public class PutFinishMarketingPlanViewModel
    {
        [Required]
        public int StaffID { get; set; }
        [Required]
        public int ID { get; set; }
        public MarketingPlan ToMarketingPlanModel()
        {
            return new MarketingPlan
            {
                ID = this.ID,
            };
        }
    }
    public class PutEditMarketingPlanViewModel
    {
        [Required]
        public int StaffID { get; set; }
        [Required]
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        public MarketingPlan ToMarketingPlanModel()
        {
            return new MarketingPlan
            {
                ID = this.ID,
                Title = this.Title,
                Description = this.Description,
                StartDate = this.StartDate,
                EndDate = this.EndDate
            };
        }

    }
    public class PostMarketingPlanViewModel
    {
        [Required]
        public int StaffID { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        public MarketingPlan ToMarketingPlanModel()
        {
            MarketingPlan _plan = new MarketingPlan();
            _plan.CreateStaffID = this.StaffID;
            _plan.Title = this.Title;
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