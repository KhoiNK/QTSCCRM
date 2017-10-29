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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int IssueID { get; set; }
        public int SalesCategoryID { get; set; }
        public bool IsDeleted { get; set; }
        [ForeignKey("IssueID")]
        public virtual Issue Issue { get; set; }
        [ForeignKey("SalesCategoryID")]
        public virtual SalesCategory SalesCategory { get; set; }
    }
}
