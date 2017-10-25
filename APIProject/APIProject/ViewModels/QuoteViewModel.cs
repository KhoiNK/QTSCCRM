using APIProject.Model.Models;
using System;
using System.Collections.Generic;
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
        public int ItemID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Unit { get; set; }

        public QuoteItemViewModel(QuoteItemMapping dto)
        {
            this.ItemID = dto.SalesItemID;
            this.Name = dto.SalesItemName;
            this.Price = dto.Price.Value;
            this.Unit = dto.Unit;
        }
    }
}