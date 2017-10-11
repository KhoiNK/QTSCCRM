using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class MarketingResultViewModel
    {
        public int ID { get; set; }
        public int? CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public int? ContactID { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int MarketingPlanID { get; set; }
        public string MarketingTitle { get; set; }
        public string Notes { get; set; }
        public int FacilityRate { get; set; }
        public int ArrangingRate { get; set; }
        public int ServicingRate { get; set; }
        public int IndicatorRate { get; set; }
        public int OthersRate { get; set; }
        public bool IsFromMedia { get; set; }
        public bool IsFromInvitation { get; set; }
        public bool IsFromWebsite { get; set; }
        public bool IsFromFriend { get; set; }
        public bool IsFromOthers { get; set; }
        public bool IsWantMore { get; set; }
        public string CreatedDate { get; set; }
        public string Status { get; set; }

        public MarketingResultViewModel(MarketingResult c)
        {
            ID = c.ID;
            CustomerName = c.CustomerName;
            CustomerAddress = c.CustomerAddress;
            ContactID = c.ContactID;
            ContactName = c.ContactName;
            Email = c.Email;
            Phone = c.Phone;
            MarketingPlanID = c.MarketingPlanID;
            MarketingTitle = c.MarketingPlan.Title;
            Notes = c.Notes;
            FacilityRate = c.FacilityRate;
            ArrangingRate = c.ArrangingRate;
            ServicingRate = c.ServicingRate;
            IndicatorRate = c.IndicatorRate;
            OthersRate = c.OthersRate;
            IsFromMedia = c.IsFromMedia;
            IsFromInvitation = c.IsFromInvitation;
            IsFromWebsite = c.IsFromWebsite;
            IsFromFriend = c.IsFromFriend;
            IsFromOthers = c.IsFromOthers;
            IsWantMore = c.IsWantMore;
            CreatedDate = c.CreatedDate;
            Status = c.Status;
        }
    }
}