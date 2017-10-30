using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Model.Models
{
    [Table("ContractItem")]
    public partial class ContractItem :BaseEntity
    {
        public int ContractID { get; set; }
        public int SalesItemID { get; set; }
        public string ItemCode { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Unit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
