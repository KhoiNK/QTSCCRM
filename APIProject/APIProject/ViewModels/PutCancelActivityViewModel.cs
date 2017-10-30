using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PutCancelActivityViewModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int StaffID { get; set; }
        public Activity ToActivityModel()
        {
            return new Activity
            {
                ID = this.ID,
                CreateStaffID = this.StaffID
            };
        }
    }
    public class PutCancelActivityResponseViewModel
    {
        public bool ActivityUpdated { get; set; }
    }
}