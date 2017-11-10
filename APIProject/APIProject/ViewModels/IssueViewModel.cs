using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class IssueViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string ContactName { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public DateTime? SolveDate { get; set; }

        public IssueViewModel(Issue dto)
        {
            this.ID = dto.ID;
            this.Title = dto.Title;
            this.ContactName = dto.Contact.Name;
            this.CustomerName = dto.Customer.Name;
            this.Status = dto.Status;
            this.SolveDate = dto.SolveDate;
        }
    }

    public class PostOpenIssueViewModel
    {
        [Required]
        public int StaffID { get; set; }
        [Required]
        public int ContactID { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? SolveDate { get; set; }
        [Required]
        public List<int> SalesCategoryIDs { get; set; }

        public Issue ToIssueModel()
        {
            return new Issue
            {
                CreateStaffID = this.StaffID,
                ContactID = this.ContactID,
                Title = this.Title,
                Description = this.Description,
                SolveDate = this.SolveDate
            };
        }
    }
    public class PostOpenIssueResponseViewModel
    {
        public bool IssueCreated { get; set; }
        public int IssueID { get; set; }
        public bool IssueUpdated { get; set; }
    }

    public class PutUpdateIssueViewModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int StaffID { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? SolveDate { get; set; }
        public Issue ToIssueModel()
        {
            return new Issue
            {
                ID=this.ID,
                Title=this.Title,
                Description=this.Description,
                SolveDate=this.SolveDate
            };
        }
    }
    public class PutUpdateIssueResponseViewModel
    {
        public bool IssueUpdated { get; set; }
    }

    public class PutDoneIssueViewModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int StaffID { get; set; }
        public Issue ToIssueModel()
        {
            return new Issue
            {
                ID=this.ID,
                SolveStaffID=StaffID
            };
        }
    }
    public class PutDoneIssueResponseViewModel
    {
        public bool IssueUpdated { get; set; }
    }
    public class PutFailIssueViewModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int StaffID { get; set; }
        public Issue ToIssueModel()
        {
            return new Issue
            {
                ID = this.ID,
                SolveStaffID=this.StaffID
            };
        }
    }
    public class PutFailIssueResponseViewModel
    {
        public bool IssueUpdated { get; set; }
    }
}