using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class ActivityDetailViewModel
    {
        public int ID { get; set; }
        //public int CustomerID { get; set; }
        //public string CustomerName { get; set; }
        //public int ContactID { get; set; }
        //public string ContactName { get; set; }
        //public string Position { get; set; }
        //public string Phone { get; set; }
        //public string Email { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Method { get; set; }
        public DateTime TodoTime { get; set; }
        public string Status { get; set; }
        public string OpportunityStage { get; set; }

        public ActivityDetailViewModel(Activity dto)
        {
            this.ID = dto.ID;
            //this.CustomerID = dto.CustomerID.Value;
            //this.CustomerName = dto.Customer.Name;
            //this.ContactID = dto.ContactID.Value;
            //this.ContactName = dto.Contact.Name;
            //this.Position = dto.Contact.Position;
            //this.Phone = dto.Contact.Phone;
            //this.Email = dto.Contact.Email;
            this.Title = dto.Title;
            this.Description = dto.Description;
            this.Type = dto.Type;
            this.Method = dto.Method;
            this.TodoTime = dto.TodoTime.Value;
            this.Status = dto.Status;
            this.OpportunityStage = dto.OfOpportunityStage;
        }
    }

    
}