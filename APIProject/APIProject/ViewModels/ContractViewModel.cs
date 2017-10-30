using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class ContractViewModel
    {
    }
    public class PostContractsViewModel
    {
        [Required]
        public int StaffID { get; set; }
        [Required]
        public int OpportunityID { get; set; }
        [Required]
        public List<PostContractViewModel> Categories { get; set; }
    }
    public class PostContractViewModel
    {
        [Required]
        public int SalesCagetogyID { get; set; }
        [Required]
        public List<ContractItemViewModel> QuoteItems { get; set; }
    }
    public struct ContractItemViewModel
    {
        [Required]
        public int QuoteItemID { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}