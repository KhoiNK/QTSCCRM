using APIProject.Model.Models;
using System;
using System.Collections.Generic;
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
        public string Stage { get; set; }
        public DateTime? ResolveDate { get; set; }

        public IssueViewModel(Issue dto)
        {
            this.ID = dto.ID;
            this.Title = dto.Title;
            this.ContactName = dto.Contact.Name;
            this.CustomerName = dto.Customer.Name;
            this.Stage = dto.Stage;
            this.ResolveDate = dto.EstimateSolveEndDate;
        }
    }
}