﻿using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PutCompleteActivityViewModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int StaffID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public List<int> CategoryIDs { get; set; }

        public Activity ToActivityModel()
        {
            return new Activity
            {
                ID = this.ID,
                CreateStaffID = this.StaffID,
                Title = this.Title,
                Description = this.Description
            };
        }
    }
}