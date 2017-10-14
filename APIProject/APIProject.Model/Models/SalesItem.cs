namespace APIProject.Model.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SalesItem")]
    public partial class SalesItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public int? SalesCategoryID { get; set; }

        public virtual SalesCategory SalesCategory { get; set; }
    }
}
