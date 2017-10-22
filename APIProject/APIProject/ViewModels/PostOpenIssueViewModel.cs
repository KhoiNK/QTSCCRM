using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using APIProject.Model.Models;
using System.ComponentModel.DataAnnotations;

namespace APIProject.ViewModels
{
    public class PostOpenIssueViewModel
    {
        [Required]
        public int StaffID { get; set; }
        [Required]
        public int ContactID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public List<int> SalesCategoryIDs { get; set; }

        public Issue ToIssueModel()
        {
            return new Issue
            {
                ModifiedStaffID = this.StaffID,
                ContactID=this.ContactID,
                Title=this.Title,
                Description=this.Description
            };
        }
    }
}