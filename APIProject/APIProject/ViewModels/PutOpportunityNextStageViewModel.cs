using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PutOpportunityNextStageViewModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int StaffID { get; set; }

        public Opportunity ToOpportunityModel()
        {
            return new Opportunity
            {
                ID = this.ID,
                CreateStaffID = this.StaffID
            };
        }
    }
}