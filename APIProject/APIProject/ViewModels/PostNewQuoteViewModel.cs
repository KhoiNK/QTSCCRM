using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PostNewQuoteViewModel
    {
        [Required]
        public int StaffID { get; set; }
        [Required]
        public int OpportunityID { get; set; }
        [Required]
        public List<int> SalesItemIDs { get; set; }
        [Range(0, 100,
            ErrorMessage = "Giá trị của thuế phải từ {1} đến {2}.")]
        public double Tax { get; set; }
        [Range(0, 100,
           ErrorMessage = "Giá trị của khuyến mãi phải từ {1} đến {2}.")]
        public double Discount { get; set; }

        public Quote ToQuoteModel()
        {
            return new Quote
            {
                CreatedStaffID = this.StaffID,
                OpportunityID = this.OpportunityID,
                Tax = this.Tax,
                Discount = this.Discount
            };
        }

    }
}