namespace APIProject.Model.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Issue")]
    public partial class Issue
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public int? CustomerID { get; set; }

        public int? ContactID { get; set; }

        public int? CreateStaffID { get; set; }

        public int? OpenStaffID { get; set; }

        public int? SolveStaffID { get; set; }

        public int? AcceptStaffID { get; set; }

        public int? ModifiedStaffID { get; set; }

        public int? SalesCategoryID { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual SalesCategory SalesCategory { get; set; }

        public virtual Staff Staff { get; set; }

        public virtual Staff Staff1 { get; set; }

        public virtual Staff Staff2 { get; set; }

        public virtual Staff Staff3 { get; set; }

        public virtual Staff Staff4 { get; set; }
    }
}
