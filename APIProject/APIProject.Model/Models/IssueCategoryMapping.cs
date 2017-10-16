namespace APIProject.Model.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IssueCategoryMapping")]
    public partial class IssueCategoryMapping
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IssueID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SalesCategoryID { get; set; }

        public bool? IsDeleted { get; set; }

        public virtual Issue Issue { get; set; }

        public virtual SalesCategory SalesCategory { get; set; }
    }
}
