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
        public int ContactID { get; set; }
        [Required]
        public int QuoteID { get; set; }
        [Required]
        public List<PostContractViewModel> Contracts { get; set; }
    }
    public class PostContractViewModel
    {
        [Required]
        public int SalesCagetogyID { get; set; }
        [Required]
        public List<ContractItemViewModel> ContractItems { get; set; }
    }
    public struct ContractItemViewModel
    {
        [Required]
        public int SalesItemID { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}