namespace APIProject.Model.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MarketingPlan")]
    public partial class MarketingPlan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MarketingPlan()
        {
            MarketingResults = new HashSet<MarketingResult>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int? CreateStaffID { get; set; }

        public int? ModifiedStaffID { get; set; }

        public int? ValidateStaffID { get; set; }

        public int? AcceptStaffID { get; set; }

        public string Title { get; set; }
        public int Budget { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Stage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Status { get; set; }
        public string ValidateNotes { get; set; }
        public string AcceptNotes { get; set; }


        public virtual Staff Staff { get; set; }

        public virtual Staff ModifiedStaff { get; set; }

        public virtual Staff Staff2 { get; set; }

        public virtual Staff Staff3 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MarketingResult> MarketingResults { get; set; }
    }
}
