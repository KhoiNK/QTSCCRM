using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Model.Models
{
    [Table("Contract")]
    public partial class Contract:BaseEntity
    {
        public int CustomerID { get; set; }
        public int ContactID { get; set; }
        public int CreatedStaffID { get; set; }
        public string ContractCode { get; set; }
        //public int SalesCategoryID { get; set; }
        public int SalesItemID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Unit { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
    }
}
