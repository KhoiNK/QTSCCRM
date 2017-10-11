﻿using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PostMarketingResultsViewModel
    {
        [Required]
        public int PlanID { get; set; }
        [Required]
        public List<PostMarketingResultViewModel> Results { get; set; }
        [Required]
        public bool IsFinished { get; set; }

        public List<MarketingResult> ToMarketingResultModels()
        {
            List<MarketingResult> _list = new List<MarketingResult>();
            foreach (PostMarketingResultViewModel item in Results)
            {
                _list.Add(new MarketingResult
                {
                    MarketingPlanID = this.PlanID,
                    CustomerID = item.CustomerID,
                    ContactID = item.ContactID,
                    CustomerName = item.CustomerName,
                    CustomerAddress = item.CustomerAddress,
                    ContactName = item.ContactName,
                    Email = item.Email,
                    Phone = item.Phone,
                    Notes = item.Notes,
                    FacilityRate = item.FacilityRate,
                    ArrangingRate = item.ArrangingRate,
                    ServicingRate = item.ServicingRate,
                    IndicatorRate = item.IndicatorRate,
                    OthersRate = item.OthersRate,
                    IsFromMedia = item.IsFromMedia,
                    IsFromInvitation = item.IsFromInvitation,
                    IsFromWebsite = item.IsFromWebsite,
                    IsFromFriend = item.IsFromFriend,
                    IsFromOthers = item.IsFromOthers,
                    IsWantMore = item.IsWantMore
                });
            };
            return _list;
        }
    }

    public class PostMarketingResultViewModel
    {
        public int? CustomerID { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string CustomerAddress { get; set; }
        public int? ContactID { get; set; }
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
        public bool IsFromMedia { get; set; }
        [Required]
        public bool IsFromInvitation { get; set; }
        [Required]
        public bool IsFromWebsite { get; set; }
        [Required]
        public bool IsFromFriend { get; set; }
        [Required]
        public bool IsFromOthers { get; set; }
        [Required]
        public bool IsWantMore { get; set; }
    }
}