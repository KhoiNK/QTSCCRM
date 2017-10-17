namespace APIProject.Model.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OpportunityCategoryMapping")]
    public partial class OpportunityCategoryMapping
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        
        public int OpportunityID { get; set; }

        
        public int SalesCategoryID { get; set; }

        public bool? IsDeleted { get; set; }

        [ForeignKey("OpportunityID")]
        public virtual Opportunity Opportunity { get; set; }
        [ForeignKey("SalesCategoryID")]
        public virtual SalesCategory SalesCategory { get; set; }
    }
}
