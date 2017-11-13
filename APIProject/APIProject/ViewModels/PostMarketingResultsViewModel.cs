using APIProject.GlobalVariables;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PostMarketingResultViewModel
    {
        [Required]
        public int PlanID { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string CustomerAddress { get; set; }
        [Required]
        public string ContactName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        public string Notes { get; set; }
        [Required]
        public int FacilityRate { get; set; }
        [Required]
        public int ArrangingRate { get; set; }
        [Required]
        public int ServicingRate { get; set; }
        [Required]
        public int IndicatorRate { get; set; }
        [Required]
        public int OthersRate { get; set; }
        [Required]
        public List<string> IsFrom { get; set; }
        [Required]
        public bool IsWantMore { get; set; }
        public MarketingResult ToResultModel()
        {
            var response = new MarketingResult()
            {
                MarketingPlanID = PlanID,
                CustomerName = this.CustomerName,
                CustomerAddress = this.CustomerAddress,
                ContactName = this.ContactName,
                Email = this.Email,
                Phone = this.Phone,
                Notes = this.Notes,
                FacilityRate = this.FacilityRate,
                ArrangingRate = this.ArrangingRate,
                ServicingRate = this.ServicingRate,
                IndicatorRate = this.IndicatorRate,
                OthersRate = this.OthersRate,
            };
            response.IsFromMedia = (IsFrom.Contains(MarketingResultIsFrom.IsFromMedia)) ? true : false;
            response.IsFromWebsite = (IsFrom.Contains(MarketingResultIsFrom.IsFromWebsite)) ? true : false;
            response.IsFromFriend = (IsFrom.Contains(MarketingResultIsFrom.IsFromFriend)) ? true : false;
            response.IsFromOthers = (IsFrom.Contains(MarketingResultIsFrom.IsFromOthers)) ? true : false;
            response.IsFromInvitation = (IsFrom.Contains(MarketingResultIsFrom.IsFromInvitation)) ? true : false;
            return response;
        }
    }

    public class MarketingResultParticipantViewModel
    {
        public int ID { get; set; }
        public int? CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public double AverageRate { get; set; }
        public bool IsWantMore { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
    }
}