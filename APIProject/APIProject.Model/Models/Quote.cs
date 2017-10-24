namespace APIProject.Model.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Quote")]
    public partial class Quote
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Quote()
        {
            QuoteItemMappings = new HashSet<QuoteItemMapping>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int? OpportunityID { get; set; }
        public int? CreatedStaffID { get; set; }

        public double Tax { get; set; }
        public double Discount { get; set; }
        public string Status { get; set; }
        public DateTime? SentCustomerDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        [ForeignKey("OpportunityID")]
        public virtual Opportunity Opportunity { get; set; }
        [ForeignKey("CreatedStaffID")]
        public virtual Staff CreatedStaff { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuoteItemMapping> QuoteItemMappings { get; set; }
    }
}
