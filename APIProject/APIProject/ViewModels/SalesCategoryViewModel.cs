using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class SalesCategoryViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<SalesItemViewModel> Items { get; set; }

        public SalesCategoryViewModel(SalesCategory dto)
        {
            this.ID = dto.ID;
            this.Name = dto.Name;
            if (dto.SalesItems.Count > 0)
            {
                Items = new List<SalesItemViewModel>();
                foreach (SalesItem _item in dto.SalesItems)
                {
                    if (_item.IsDelete == false)
                    {
                        Items.Add(new SalesItemViewModel(_item));
                    }
                }
            }
        }
    }

    public class OpportunityCategoryViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<SalesItemViewModel> Items { get; set; }
    }

}