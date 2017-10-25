namespace APIProject.Model.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Opportunity")]
    public partial class Opportunity
    {
        public Opportunity()
        {
            Activities = new HashSet<Activity>();
            OpportunityCategoryMappings = new HashSet<OpportunityCategoryMapping>();
            Quotes = new HashSet<Quote>();

        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int? CustomerID { get; set; }
        //public int? StageID { get; set; }


        public int? ContactID { get; set; }

        public int? CreateStaffID { get; set; }

        public int? ModifyStaffID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ConsiderStart { get; set; }
        public DateTime? MakeQuoteStart { get; set; }
        public DateTime? ValidateQuoteStart { get; set; }
        public DateTime? SendQuoteStart { get; set; }   
        public DateTime? NegotiationStart { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime? LastModified { get; set; }
        public string StageName { get; set; }
        public int Priority { get; set; }
        //public string StageDescription { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual Customer Customer { get; set; }
        [ForeignKey("CreateStaffID")]
        public virtual Staff CreatedStaff { get; set; }

        public virtual Staff ModifyStaff { get; set; }
        //[ForeignKey("StageID")]
        //public virtual Stage Stage { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Activity> Activities { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpportunityCategoryMapping> OpportunityCategoryMappings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quote> Quotes { get; set; }

    }
}
