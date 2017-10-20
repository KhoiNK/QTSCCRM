using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PostOpportunityViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public List<int> CategoryIDs { get; set; }

        public Opportunity ToOpportunityModel()
        {
            return new Opportunity
            {
                Title = this.Title,
                Description = this.Description
            };

        }
    }
}