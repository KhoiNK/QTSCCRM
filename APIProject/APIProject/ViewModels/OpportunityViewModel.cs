using APIProject.GlobalVariables;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class OpportunityViewModel
    {
        public int ID { get; set; }
        public string CustomerAvatarUrl { get; set; }
        public string CustomerName { get; set; }
        public string Title { get; set; }
        public string StageName { get; set; }
        public DateTime? NextActivityTime { get; set; }
        public string Owner { get; set; }

        public OpportunityViewModel(Opportunity dto)
        {
            this.ID = dto.ID;
            this.CustomerAvatarUrl = dto.Customer.AvatarSrc;
            this.CustomerName = dto.Customer.Name;
            this.Title = dto.Title;
            this.StageName = dto.StageName;
            var foundLastOngoingActivity = dto.Activities.Where(c =>  c.Type == ActivityType.ToCustomer
            && (c.Status == ActivityStatus.Open || c.Status == ActivityStatus.Overdue)).LastOrDefault();
            if(foundLastOngoingActivity != null)
            {
                NextActivityTime = foundLastOngoingActivity.TodoTime.Value;
            }
            Owner = dto.CreatedStaff.Name;
        }
    }
}