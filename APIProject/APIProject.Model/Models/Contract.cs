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
        public int SalesCategoryID { get; set; }
        public string ContractCode { get; set; }
        public string Status { get; set; }
    }
}
