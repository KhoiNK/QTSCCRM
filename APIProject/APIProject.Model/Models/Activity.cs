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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public int? CreateStaffID { get; set; }

        public int? OfStaffID { get; set; }

        public int? CustomerID { get; set; }

        public int? ContactID { get; set; }

        public int? ModifiedStaffID { get; set; }
        public int? OpportunityID { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Method { get; set; }
        public DateTime? Deadline { get; set; }
        public string Note { get; set; }

        public virtual Opportunity Opportunity { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual Staff Staff { get; set; }

        public virtual Staff Staff1 { get; set; }

        public virtual Staff Staff2 { get; set; }
    }
}
