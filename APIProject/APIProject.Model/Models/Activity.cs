namespace APIProject.Model.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Activity")]
    public partial class Activity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int? CreateStaffID { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? OfStaffID { get; set; }

        public int? CustomerID { get; set; }

        public int? ContactID { get; set; }

        public int? ModifiedStaffID { get; set; }
        public int? OpportunityID { get; set; }
        public string OfOpportunityStage { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        //public bool IsFromCustomerType { get; set; }
        public string Method { get; set; }
        public DateTime? TodoTime { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        [ForeignKey("OpportunityID")]
        public virtual Opportunity Opportunity { get; set; }
        [ForeignKey("ContactID")]
        public virtual Contact Contact { get; set; }
        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }

        public virtual Staff Staff { get; set; }

        public virtual Staff Staff1 { get; set; }

        public virtual Staff Staff2 { get; set; }
    }
}
