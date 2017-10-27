﻿using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PutOpportunityInformationViewModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int StaffID { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public List<int> CategoryIDs { get; set; }

        public Opportunity ToOpportunityModel()
        {
            return new Opportunity
            {
                ID = this.ID,
                UpdatedStaffID = this.StaffID,
                Title = this.Title,
                Description = this.Description
            };
        }
    }
}