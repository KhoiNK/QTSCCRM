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
        public List<QuoteItemViewModel> Items { get; set; }
        public double Tax { get; set; }
        public double Discount { get; set; }
        public string Status { get; set; }

        public QuoteViewModel(Quote dto)
        {
            this.ID = dto.ID;
            this.Tax = dto.Tax;
            this.Discount = dto.Discount;
            this.Status = dto.Status;
            if (dto.QuoteItemMappings.Where(c => c.IsDeleted == false).Count() != 0)
            {
                var quoteItems = dto.QuoteItemMappings.Where(c => c.IsDeleted == false);
                Items = new List<QuoteItemViewModel>();
                foreach (QuoteItemMapping item in quoteItems)
                {
                    Items.Add(new QuoteItemViewModel(item));
                }
            }
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