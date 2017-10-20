using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PostSalesItemViewModel
    {
        [Required]
        public int CategoryID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public string Unit { get; set; }

        public SalesItem ToSalesItemModel()
        {
            return new SalesItem
            {
                SalesCategoryID = this.CategoryID,
                Name = this.Name,
                Price = this.Price,
                Unit = this.Unit
            };
        }
    }
}