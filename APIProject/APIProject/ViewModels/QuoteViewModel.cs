using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class QuoteViewModel
    {
        public int ID { get; set; }
        public double Tax { get; set; }
        public double Discount { get; set; }
        public string Status { get; set; }
        public StaffDetailViewModel ValidatedStaff { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? SentCustomerDate { get; set; }
        public List<QuoteItemViewModel> Items { get; set; }


        public QuoteViewModel(Quote dto)
        {
            this.ID = dto.ID;
            this.Tax = dto.Tax;
            this.Discount = dto.Discount;
            this.Status = dto.Status;
            this.SentCustomerDate = dto.SentCustomerDate;
            if (dto.QuoteItemMappings.Where(c => c.IsDelete == false).Count() != 0)
            {
                var quoteItems = dto.QuoteItemMappings.Where(c => c.IsDelete == false);
                Items = new List<QuoteItemViewModel>();
                foreach (QuoteItemMapping item in quoteItems)
                {
                    Items.Add(new QuoteItemViewModel(item));
                }
            }
            if (dto.ValidatedStaffID.HasValue)
            {
                this.ValidatedStaff = new StaffDetailViewModel(dto.ValidatedStaff);
            }
            this.UpdatedDate = dto.UpdatedDate;
        }
    }

    public class QuoteItemViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Unit { get; set; }

        public QuoteItemViewModel(QuoteItemMapping dto)
        {
            this.ID = dto.ID;
            this.Name = dto.SalesItemName;
            this.Price = dto.Price.Value;
            this.Unit = dto.Unit;
        }
    }
    public class QuoteDetailsViewModel
    {
        public int ID { get; set; }
        public double Tax { get; set; }
        public double Discount { get; set; }
        public string Status { get; set; }
        public StaffDetailViewModel ValidatedStaff { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? SentCustomerDate { get; set; }
        public List<QuoteCategotyViewModel> Categories { get; set; }
    }

    public struct QuoteCategotyViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<QuoteItemViewModel> Items { get; set; }
    }

    public class PutUpdateQuoteViewModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int StaffID { get; set; }
        [Range(0, 100,
            ErrorMessage = "Giá trị của thuế phải từ {1} đến {2}.")]
        public double Tax { get; set; }
        [Range(0, 100,
           ErrorMessage = "Giá trị của khuyến mãi phải từ {1} đến {2}.")]
        public double Discount { get; set; }
        [Required]
        public List<int> SalesItemIDs { get; set; }

        public Quote ToQuoteModel()
        {
            return new Quote
            {
                ID=this.ID,
                CreatedStaffID=this.StaffID,
                Tax=this.Tax,
                Discount=this.Discount
            };
        }
    }
    public class PutUpdateQuoteResponseViewModel
    {
        public bool BasicInfoUpdated { get; set; }
        public bool QuoteItemsUpdated { get; set; }
    }

    public class PutValidQuoteViewModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int StaffID { get; set; }
        public string Notes { get; set; }
        public Quote ToQuoteModel()
        {
            return new Quote
            {
                ID = this.ID,
                ValidatedStaffID = this.StaffID,
                Notes = this.Notes
            };
        }
    }
    public class PutValidQuoteResponseViewModel
    {
        public bool QuoteUpdated { get; set; }
        public bool OpportunityUpdated { get; set; }
    }
    public class PutInValidQuoteViewModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int StaffID { get; set; }
        public string Notes { get; set; }
        public Quote ToQuoteModel()
        {
            return new Quote
            {
                ID = this.ID,
                ValidatedStaffID = this.StaffID,
                Notes = this.Notes,
            };
        }
    }
    public class PutInvalidQuoteResponseViewModel
    {
        public bool QuoteUpdated { get; set; }
        public bool OpportunityUpdated { get; set; }
    }

    public class PutSendQuoteViewModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int StaffID { get; set; }
        public Quote ToQuoteModel()
        {
            return new Quote
            {
                ID = this.ID,
                CreatedStaffID = this.StaffID
            };
        }
    }
    public class PutSendQuoteResponseViewModel
    {
        public bool QuoteSent { get; set; }
        public bool OpportunityUpdated { get; set; }
    }
}