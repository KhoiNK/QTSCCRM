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
        public int ContactID { get; set; }
        public int? OpportunityID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Method { get; set; }
        [Required]
        public DateTime TodoTime { get; set; }
        public List<int> CategoryIDs { get; set; }

        public Activity ToActivityModel()
        {
            Activity activity = new Activity
            {
                CreateStaffID = this.StaffID,
                ContactID = this.ContactID,
                Title = this.Title,
                OpportunityID = this.OpportunityID,
                Description = this.Description,
                Type = this.Type,
                Method = this.Method,
                TodoTime = this.TodoTime
            };

            return activity;
           
        }
    }

    
}