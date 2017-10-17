using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PostNewActivityViewModel
    {
        [Required]
        public int StaffID { get; set; }
        [Required]
        public int CustomerID { get; set; }
        [Required]
        public int ContactID { get; set; }
        public int? OpportunityID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Note { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Method { get; set; }
        [Required]
        public DateTime TodoTime { get; set; }

        public Activity ToActivityModel()
        {
            Activity activity = new Activity();

            activity.ModifiedStaffID = this.StaffID;
            activity.CustomerID = this.CustomerID;
            activity.ContactID = this.ContactID;
            activity.Title = this.Title;
            activity.OpportunityID = this.OpportunityID;
            activity.Note = this.Note;
            activity.Type = this.Type;
            activity.Method = this.Method;
            activity.TodoTime = this.TodoTime;

            return activity;
                //todo
           
        }
    }
}