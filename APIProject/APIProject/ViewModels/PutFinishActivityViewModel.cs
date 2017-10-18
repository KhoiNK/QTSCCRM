﻿using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PutFinishActivityViewModel
    {
        [Required]
        public int StaffID { get; set; }
        [Required]
        public int ActivityID { get; set; }


        public Activity ToActivityModel()
        {
            return new Activity
            {
                ModifiedStaffID = this.StaffID,
                ID = this.ActivityID
            };
        }

    }
}