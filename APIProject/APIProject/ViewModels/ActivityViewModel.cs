using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class ActivityViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Method { get; set; }
        public string Owner { get; set; }
        public DateTime TodoTime { get; set; }
        public string Status { get; set; }
        public string OfOpportunityStage { get; set; }

        public ActivityViewModel(Activity dto)
        {
            this.ID = dto.ID;
            this.Title = dto.Title;
            this.Type = dto.Type;
            this.Method = dto.Method;
            this.Owner = dto.Staff.Name;
            this.TodoTime = dto.TodoTime.Value;
            this.Status = dto.Status;
            this.OfOpportunityStage = dto.OfOpportunityStage;
        }
    }
}